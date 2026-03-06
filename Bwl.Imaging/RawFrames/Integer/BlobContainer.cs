namespace Bwl.Imaging
{
    public class IntegerBlob
    {
        public string ID;
        public int[] Data;

        public void WriteDataToStream(FileStream fs)
        {
            byte[] bytes;
            for (int i = 0, loopTo = Data.Length - 1; i <= loopTo; i++)
            {
                bytes = BitConverter.GetBytes(Data[i]);
                fs.Write(bytes, 0, bytes.Length);
            }
        }

        public void ReadDataFromStream(FileStream fs, int datalength)
        {
            Data = new int[datalength];
            byte[] bytes = BitConverter.GetBytes(0);
            for (int i = 0, loopTo = Data.Length - 1; i <= loopTo; i++)
            {
                fs.Read(bytes, 0, bytes.Length);
                Data[i] = BitConverter.ToInt32(bytes, 0);
            }
        }
    }

    public class BlobContainer
    {
        public Dictionary<string, string> Attributes { get; private set; } = new Dictionary<string, string>();
        public List<IntegerBlob> Blobs { get; private set; } = new List<IntegerBlob>();

        public BlobContainer()
        {
        }

        public BlobContainer(BlobContainer bc)
        {
            Blobs = bc.Blobs;
            Attributes = bc.Attributes;
        }

        public void Save(string filename)
        {
            using (var fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                foreach (var attr in Attributes)
                    WriteLineToStream(fs, $"{attr.Key}={attr.Value}\r\n");
                foreach (var Blob in Blobs)
                {
                    WriteLineToStream(fs, $"{{BlobStart}}={Blob.ID},{Blob.Data.GetType()},{Blob.Data.Length}\r\n");
                    Blob.WriteDataToStream(fs);
                    WriteLineToStream(fs, $"{{BlobEnd}}={Blob.ID}\r\n");
                }
                WriteLineToStream(fs, $"{{End}}={{End}}\r\n");
                fs.Flush();
                fs.Close();
            }
        }

        public static BlobContainer FromFile(string filename)
        {
            using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                var @file = new BlobContainer();
                do
                {
                    string[] keyvalue = ReadLineFromStream(fs).Split('=');
                    if (keyvalue.Length > 1)
                    {
                        string key = keyvalue[0];
                        string value = keyvalue[1];
                        bool exitDo = false;
                        switch (key ?? "")
                        {
                            case "{BlobStart}":
                                {
                                    string[] @params = value.Split(',');
                                    if (@params.Length != 3)
                                        throw new Exception("Bad file format");
                                    string id = @params[0];
                                    string typename = @params[1];
                                    int length = Convert.ToInt32(@params[2]);
                                    switch (typename ?? "")
                                    {
                                        case "System.Int32[]":
                                            {
                                                var blob = new IntegerBlob();
                                                blob.ReadDataFromStream(fs, length);
                                                blob.ID = id;
                                                @file.Blobs.Add(blob);
                                                string[] endline = ReadLineFromStream(fs).Split('=');
                                                if (endline[0] != "{BlobEnd}")
                                                    throw new Exception("Bad BlobContainer file: BLOB " + id + " not finished with {BlobEnd}");
                                                break;
                                            }

                                        default:
                                            {
                                                throw new Exception("Bad BlobContainer file: unsupported BLOB type:" + typename);
                                            }
                                    }

                                    break;
                                }
                            case "{End}":
                                {
                                    exitDo = true;
                                    break;
                                }

                            default:
                                {
                                    @file.Attributes.Add(key, value);
                                    break;
                                }
                        }

                        if (exitDo)
                        {
                            break;
                        }
                    }
                }
                while (true);
                return @file;
            }
        }

        private static void WriteLineToStream(FileStream fs, string line)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(line);
            fs.Write(bytes, 0, bytes.Length);
        }

        private static string ReadLineFromStream(FileStream fs)
        {
            var bytes = new List<byte>();
            int read = fs.ReadByte();
            while (read == 10 | read == 13)
                read = fs.ReadByte();
            while (read != 10 & read != 13 & read != 0x0)
            {
                bytes.Add((byte)read);
                read = fs.ReadByte();
            }
            return System.Text.Encoding.UTF8.GetString(bytes.ToArray());
        }
    }
}