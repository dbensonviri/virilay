using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
namespace ConfigManager
{
    class ReflectionSerializer
    {
        private Dictionary<Type, MethodInfo> _convert = new Dictionary<Type, MethodInfo>();
        
        public ReflectionSerializer()
        {
            MethodInfo[] infos = this.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public );
            foreach (MethodInfo mi in typeof(IConvertible).GetMethods(BindingFlags.Instance | BindingFlags.Public))
            {
                if (mi.Name.Substring(0, 2) == "To")
                {
                    _convert.Add(mi.ReturnType, mi);
                    continue;
                }

                if (mi.GetParameters().Length != 0)
                    continue;
            }
        }
        public void WriteFields(string filename,object o)
        {
            List<string>lines=new List<string>();
            foreach (FieldInfo field in o.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public ))
                lines.Add(field.Name + "=" + field.GetValue(o));
            foreach (PropertyInfo pi in o.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                lines.Add(pi.Name + "=" + pi.GetValue(o));
                
            }
            Virilib.WriteFile(filename, lines);
        }
        public bool ReadFields<T>(string filename, T o)
        {
            if (!File.Exists(filename))
                return false;
            
            Dictionary<string, FieldInfo> fields = new Dictionary<string, FieldInfo>();
            Dictionary<string, PropertyInfo> properties = new Dictionary<string, PropertyInfo>();
            foreach (FieldInfo field in o.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
                fields.Add(field.Name, field);
            foreach (PropertyInfo pi in o.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                properties.Add(pi.Name, pi);
            
            Virilib.ReadFile(filename, (word) =>
                {
                    if (word.Count == 2)
                    {
                        if (fields.ContainsKey(word[0]))
                        {
                            IConvertible iConv = word[1];
                            
                            object converted = _convert[fields[word[0]].FieldType].Invoke(iConv, new object[] {null });
                            string fieldname = word[0]; 
                            fields[fieldname].SetValue(o, converted); //eg. fields["Username"].SetValue(o,"my username");
                            if (word[0] == "Username")
                            {
                                Console.WriteLine(fields[fieldname].GetValue(o));
                                int a = 1;
                            }


                        }
                        if (properties.ContainsKey(word[0]))
                        {
                            IConvertible iConv = word[1];
                            properties[word[0]].SetValue(o, _convert[properties[word[0]].PropertyType].Invoke(iConv, new object[] {  }));
                            Console.WriteLine(word[1]);                            
                        }
                    }
                });
            return true;
        }
    }
}
