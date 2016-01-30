using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Config
{
    public class IniFile
    {
        private readonly IniFileItemRecorder itemRecorder;
        internal IEnumerable<IniFileItem> Items
        {
            get { return itemRecorder != null ? itemRecorder.Items : null; }
        }

        // ini file object
        public IniFile(string path)
        {
            if (!File.Exists(path))
            {
                Console.WriteLine("IniFile not found path={0}", path);
                return;
            }

            #region Read Ini File
            var sr = new StreamReader(path, Encoding.Default);
            string line;
            string newSection = "";
            var items = new List<IniFileItem>();

            while ((line = sr.ReadLine()) != null)
            {
                line = line.Trim();
                if (line.IndexOf("//") >= 0) //remove commont
                {
                    line = line.Substring(0, line.IndexOf("//"));
                }
                if (line.Length > 0)
                {
                    if (line[0] == '[') //take as a section
                    {
                        int start = 0;
                        int end = line.IndexOf(']');
                        if (end > start)
                        {
                            newSection = line.Substring(start + 1, end - start - 1).Trim();
                        }
                    }
                    else if (line.IndexOf('=') >= 0) //task as a key
                    {
                        string[] infos = line.Split('=');
                        if (infos.Length == 2)
                        {
                            IniFileItem item = new IniFileItem();
                            item.section = newSection;
                            item.keyword = infos[0].Trim();
                            item.value = infos[1].Trim();
                            items.Add(item);
                        }
                    }
                }
            }
            sr.Close();
            itemRecorder = new IniFileItemRecorder(items);
            #endregion
        }
    }

    internal struct IniFileItem
    {
        public string section;
        public string keyword;
        public string value;

        public override string ToString()
        {
            return string.Format("{0}:{1}:{2}", section, keyword, value);
        }
    }

    internal class IniFileItemRecorder
    {
        readonly List<IniFileItem> items = new List<IniFileItem>();
        internal IEnumerable<IniFileItem> Items
        {
            get { return items; }
        }

        public IniFileItemRecorder(List<IniFileItem> items)
        {
            this.items = items;
        }

        public string Read(string section, string keyword)
        {
            foreach (var item in items)
            {
                if (item.section == section && item.keyword == keyword)
                {
                    return item.value;
                }
            }

            throw new Exception(string.Format("IniRead {0}/{1} miss", section, keyword));
        }

        internal static class ConvertHelper<T>
        {
            public delegate T ConverterFunc(string value);
            public static ConverterFunc Converter;
            public static T Convert(string value)
            {
                if (Converter == null)
                {
                    throw new Exception(string.Format("not support config item type type={0}", typeof(T)));
                }
                return Converter(value);
            }
        }

        internal static List<T> ConvertList<T>(string value)
        {
            var rets = value.Split('|');
            var list = new List<T>();

            foreach (var ret in rets)
            {
                list.Add(ConvertHelper<T>.Convert(ret));
            }

            return list;
        }
        static KeyValuePair<TFirst, TSecond> ConvertPair<TFirst, TSecond>(string value)
        {
            var rets = value.Split(',');

            return new KeyValuePair<TFirst, TSecond>(ConvertHelper<TFirst>.Convert(rets[0]), ConvertHelper<TSecond>.Convert(rets[1]));
        }

        static EndPoint ConvertEndPoint(string value)
        {
            var rets = value.Split(':');
            var host = rets[0];
            var port = rets[1];

            IPAddress ip;
            if (IPAddress.TryParse(host, out ip))
            {
                return new IPEndPoint(ip, int.Parse(port));
            }
            else
            {
                //return new DnsEndPoint(host, port);
                throw new Exception(string.Format("invalid IPEndPoint format {0}", value));
            }
        }

        static IniFileItemRecorder()
        {
            ConvertHelper<int>.Converter = Convert.ToInt32;
            ConvertHelper<uint>.Converter = Convert.ToUInt32;
            ConvertHelper<bool>.Converter = Convert.ToBoolean;
            ConvertHelper<float>.Converter = Convert.ToSingle;
            ConvertHelper<string>.Converter = value => value;
            ConvertHelper<EndPoint>.Converter = ConvertEndPoint;
            ConvertHelper<KeyValuePair<string, int>>.Converter = ConvertPair<string, int>;
            ConvertHelper<List<KeyValuePair<string, int>>>.Converter = ConvertList<KeyValuePair<string,int>>;
            ConvertHelper<List<EndPoint>>.Converter = ConvertList<EndPoint>;
        }

        public T Read<T>(string section, string keyword)
        {
            string d = Read(section, keyword);
            if (d == "")
            {
                throw new Exception(string.Format("IniRead {0}/{1} is invalid", section, keyword));
            }

            return ConvertHelper<T>.Convert(d);
        }
    }
}
