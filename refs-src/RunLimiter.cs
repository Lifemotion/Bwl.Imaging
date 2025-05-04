//   Copyright 2019-2024 Artem Drobanov (artem.drobanov@gmail.com)

//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may Not use this file except In compliance With the License.
//   You may obtain a copy Of the License at

//     http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law Or agreed To In writing, software
//   distributed under the License Is distributed On an "AS IS" BASIS,
//   WITHOUT WARRANTIES Or CONDITIONS Of ANY KIND, either express Or implied.
//   See the License For the specific language governing permissions And
//   limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Periodic code run limiter with automatic internal memory clean.
/// </summary>
public class RunLimiter
{
	private TimeSpan _period;
	private TimeSpan _memoryDepth;
	private int _memorySizeMax;
	private RunLimiter _rlClean;
	private Dictionary<string, DateTime> _runs = new Dictionary<string, DateTime>();
	private object _syncRoot = new object();

	public int Count
	{
		get
		{
			lock (_syncRoot)
			{
				return _runs.Count;
			}
		}
	}

	public TimeSpan Period
	{
		get
		{
			return _period;
		}
	}

	public RunLimiter(TimeSpan period,
					  double memoryDepthPeriods = 2,
					  int memorySizeMax = 10000,
					  double memoryCleanPeriodMs = 1000) : this(period.TotalMilliseconds,
																memoryDepthPeriods,
																memorySizeMax,
																memoryCleanPeriodMs)
	{
	}

	public RunLimiter(double periodMs = 1000,
					  double memoryDepthPeriods = 2,
					  int memorySizeMax = 10000,
					  double memoryCleanPeriodMs = 1000)
	{
		_period = TimeSpan.FromMilliseconds(periodMs);
		_memoryDepth = TimeSpan.FromMilliseconds(periodMs * memoryDepthPeriods);
		_memorySizeMax = memorySizeMax;
		_rlClean = (memoryCleanPeriodMs >= 0) ? new RunLimiter(periodMs: memoryCleanPeriodMs, memoryCleanPeriodMs: -1) : null; // Memory clean run limiter has no it's own memory cleaning
	}

	public bool Run(Action action, string actionId = "", bool suppressExceptions = false)
	{
		lock (_syncRoot)
		{
			bool result = false;
			if (!_runs.ContainsKey(actionId)) // New elem...
			{
				if (_runs.Count > _memorySizeMax) //...means memory size check
				{
					_rlClean?.Run(() =>
					{
						var utcNow = DateTime.UtcNow;
						foreach (var kvp in _runs.ToArray())
						{
							if ((utcNow - kvp.Value).Duration() > _memoryDepth)
							{
								_runs.Remove(kvp.Key);
							}
						}
					}); // Memory clean should't happends very often
				}
				_runs.Add(actionId, DateTime.MinValue); // New elem initially has default timestamp
			}
			if ((DateTime.UtcNow - _runs[actionId]).Duration() >= _period) // Next recurring run
			{
				try
				{
					action(); // RUN
				}
				catch (Exception ex)
				{
					if (!suppressExceptions)
					{
						throw ex;
					}
				}
				finally
				{
					_runs[actionId] = DateTime.UtcNow; // End of activity - the beginning of inactivity period
				}
				result = true;
			}
			return result;
		}
	}
}
