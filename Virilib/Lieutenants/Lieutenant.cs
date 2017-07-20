using Microsoft.CSharp;
using System.Threading.Tasks;
using System.Diagnostics;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.ComponentModel; // for TypeConverter
namespace Viri.Lieutenants
{

    /// <summary>
    /// You give your Executor some Lieutenants that is responsible for something (!compile math)
    /// You issues orders to the Executor (such as !sin 30, or !math.sin 30).
    /// The Executor assigns the task to the appropriate Lieutenant. If multiple lieutenants can run it, it must be specified
    /// Usage:
    /// Executor exec=new Executor("Me",""); //username is me, workingdirectory is default
    /// exec.Compile("Math"); //compiles Math.dll in the working directory
    /// public bool Execute(string inputstring, AdditionalParameters additionalparms, out object[] refvals)
    /// object[]result; //returns, refs, and outs are stored in this vector
    /// AdditionalParameters ap=new AdditionalParameters();
    /// ap.Origin="virifaux";       //run this as virifaux. AdditionalParameters is a partial class, so you can add more
    /// if (exec.Execute("math.sin 30",ap,out result))
    /// {
    ///     Console.Writeline("Returns "+result[0]);
    /// }
    /// </summary>
    public class Executor : IDisposable
    {
        sealed class Lieutenant
        {
            public object CSObject;
            public Dictionary<string, List<MethodInfo>> Methods = new Dictionary<string, List<MethodInfo>>();

            public Lieutenant(object responsible)
            {
                CSObject = responsible;
                MethodInfo[] methods = responsible.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Static);
                foreach (MethodInfo mi in methods)
                {
                    string name = mi.Name.ToLower();
                    if (!Methods.ContainsKey(name)) Methods.Add(name, new List<MethodInfo>());
                    Methods[name].Add(mi);
                }

            }

            //retvalues[0] is the return
            //retvalues[1..n] are ref/outs
            [Help("Executes the function")]
            public bool Execute(string method, string[] words, AdditionalParameters additionalparms, out object[] refvals)
            {
                refvals = new object[1] { "" };
                if (Methods.ContainsKey(method))
                {
                    foreach (MethodInfo mi in Methods[method])
                    {
                        ParameterInfo[] miparameters = mi.GetParameters();
                        object[] parametervalues = new object[miparameters.Length];
                        AdditionalParametersAttribute apa = mi.GetCustomAttribute<AdditionalParametersAttribute>(); //todo: deprecate
                        string s = "";
                        foreach (ParameterInfo pi in miparameters)
                            s += pi.ParameterType + ",";
                        if (miparameters.Length != words.Length + (apa == null ? 0 : 1)) continue;
                        int wordsindex = -1;
                        bool gotgoodparameters = true;
                        for (int i = 0; i < miparameters.Length; i++) //of method parameters //we convert the parameters to the true datatype. "6"=>6, if the types match
                        {
                            Type dereferencedtype;
                            if (miparameters[i].ParameterType.IsByRef || miparameters[i].IsOut)
                                dereferencedtype = miparameters[i].ParameterType.GetElementType();
                            else
                                dereferencedtype = miparameters[i].ParameterType;
                            if (miparameters[i].ParameterType == typeof(AdditionalParameters))
                                parametervalues[i] = additionalparms;
                            else if (++wordsindex < words.Length && CanConvert(words[wordsindex], dereferencedtype))
                                parametervalues[i] = _convert[dereferencedtype].Invoke(words[wordsindex] as IConvertible, new object[] { null });
                            else
                            {
                                gotgoodparameters = false;
                                break;
                            }
                        }

                        if (!gotgoodparameters)
                            continue;
                        
                        if (mi.GetCustomAttribute<ViewerPermittedAttribute>() == null && additionalparms.Origin != Executor.Owner)
                        {
                            refvals[0] = "Only owner can run this";
                            return false;
                        }
                        //we have a matching set of parameters, converted to their true datatype. now we invoke the method
                        try
                        {
                            if (mi.ReturnType == typeof(void))
                            {
                                Task.FromResult(mi.Invoke(CSObject, parametervalues));
                                List<object> refvalueslist = new List<object>();
                                refvalueslist.Add(null); //void
                                for (int i = 0; i < miparameters.Length; i++)
                                {
                                    if (miparameters[i].IsOut || miparameters[i].ParameterType.IsByRef)
                                        refvalueslist.Add(parametervalues[i]);

                                }
                                refvals = refvalueslist.ToArray();

                                return true;
                            }
                            else// if (mi.ReturnType !=typeof(Task))
                            {

                                //output =Task.Factory.StartNew(()=>{return mi.Invoke(_methods[words[0]].O, parameters)}).ToString();// ();
                                //string output = await Task.FromResult<string>(mi.Invoke(_methods[words[0]].O, parametervalues).ToString());
                                refvals[0] = Task.FromResult(mi.Invoke(CSObject, parametervalues)).Result;
                                List<object> refvalueslist = new List<object>();
                                refvalueslist.Add(refvals[0]);
                                int index = 0;
                                foreach (ParameterInfo pi in miparameters)
                                {
                                    if (pi.IsOut || pi.ParameterType.IsByRef)
                                    {
                                        refvalueslist.Add(parametervalues[index]);
                                    }
                                    index++;
                                }
                                refvals = refvalueslist.ToArray();
                                return true;
                            }

                        }

                        catch (Exception e)
                        {
                            Log(e.ToString(), "EXECUTE");
                            refvals[0] = "Exception logged at " + _logname;
                            return false;
                        }

                    }
                    
                }
                else
                {
                    refvals[0] = "No methods for " + method;
                    return false;
                }
                //we tried to match parameters to each method, but didn't find one. so report that we didnt find matching parameter set
                StringBuilder op = new StringBuilder();
                op.Append("Mismatched parameters.");
                if (words.Length >= 1)
                    op.AppendFormat("{0}", words[0]);
                for (int i = 1; i < words.Length; i++)
                    op.AppendFormat(", {0}", words[i]);
                refvals[0] = op.ToString() + " " + Help(method);
                return false;
            }
           
            public string Help(string command)
            {
                StringBuilder sb = new StringBuilder();
                if (!Methods.ContainsKey(command))
                    return command + " not found in " + CSObject.GetType().Name;
                foreach (MethodInfo mi in Methods[command])
                {
                    HelpAttribute ha = mi.GetCustomAttribute<HelpAttribute>();
                    if (ha != null)
                    {
                        sb.Append(ha.HelpText+"\n");
                    }
                    ParameterInfo[] pi = mi.GetParameters();
                    sb.AppendFormat("{0} {1}(", mi.ReturnType.Name.ToLower(), mi.Name);
                    if (pi.Length != 0)
                        sb.AppendFormat("{0} {1}", pi[0].ParameterType.Name.ToLower(), pi[0].Name);
                    for (int i = 1; i < pi.Length; i++)
                        sb.AppendFormat(", {0} {1}", pi[i].ParameterType.Name.ToLower(), pi[i].Name);
                    sb.Append(")");
                    List<object> attrs = mi.GetCustomAttributes(false).ToList();
                    if (attrs.Count > 0)
                    {
                        sb.AppendFormat("\n[{0}", attrs[0].GetType().Name);
                        for (int i = 1; i < attrs.Count; i++)
                            sb.AppendFormat(",{0}", attrs[i].GetType().Name);
                        sb.Append("]");
                    }
                    
                }
                return sb.ToString();
            }

        }

        #region Custom Datatypes


        #endregion
        #region Data

        List<string> _disallowedcommands = new List<string>(); //affected by allow

        public static string Owner = "";

        //each lieutenant points to an object. _lieutenants["Math"] points to the lieutenant that is repsonsible for calling math functions
        Dictionary<string, Lieutenant> _lieutenants = new Dictionary<string, Lieutenant>();

        //if you call execute "Say", 
        //If there is only one class that does "Say", execute _methodtolieutenant["Say"].Execute("Say");
        Dictionary<string, List<Lieutenant>> _methodtolieutenant = new Dictionary<string, List<Lieutenant>>();

        //Allows conversions between strings and other types. For instance, _convert[Int32] calls ToInt32(...)
        protected static Dictionary<Type, MethodInfo> _convert = new Dictionary<Type, MethodInfo>(); // type -> convert. Used to easily convert strings into other types

        #endregion

        #region Lifetimes

        public void AddLieutenant(object uses)
        {
            string classname = uses.GetType().Name.ToLower();
            if (_lieutenants.ContainsKey(classname))
            {
                Console.WriteLine(classname + " already exists");
                return;
            }
            Lieutenant ret = new Lieutenant(uses);

            _lieutenants.Add(classname, ret);
            foreach (List<MethodInfo> LMI in ret.Methods.Values)
                foreach (MethodInfo mis in LMI)
                {
                    string name = mis.Name.ToLower();
                    if (!_methodtolieutenant.ContainsKey(name))
                        _methodtolieutenant.Add(name, new List<Lieutenant>());
                    if (!_methodtolieutenant[name].Contains(ret))
                        _methodtolieutenant[name].Add(ret);
                }

        }

        string DIRECTORY="";
        public Executor(string owner,string directory)
        {
            Owner = owner;
            DIRECTORY = directory;
            if (!Directory.Exists(DIRECTORY + "Logs"))
            {
                Directory.CreateDirectory(DIRECTORY + "Logs");
            }
            AddLieutenant(this);

            foreach (MethodInfo mi in typeof(IConvertible).GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                if (mi.Name.Substring(0, 2) == "To")
                    _convert.Add(mi.ReturnType, mi);
            }
        }
        
        

        [Help("Shows the help text. eg.Help('splat'), Help('interactive.splat')")] //stored in the helpattribute
        [ViewerPermitted]
        public string Help(string command)
        {
            Lieutenant lt;
            string methodname;
            string message;
            if (!_dotnotation(command, out lt, out methodname, out message))
                return message;
            return lt.Help(methodname);

        }

  
        /// <summary>
        /// Converts an input (eg. "math.sin" or "sin") into its lt and method (in this case, math lieuteneant and "sin")
        /// Returns true if found class and method
        /// Returns false otherwise, with a message explaining what went wrong
        /// </summary>
        /// <param name="input"></param>
        /// <param name="lt"></param>
        /// <param name="methodname"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        bool _dotnotation(string input, out Lieutenant lt, out string methodname, out string message)
        {
            message = "";
            input = input.ToLower();
            lt = null;
            methodname = "";
            int indexofdot = input.IndexOf('.');
            if (indexofdot==input.Length-1)
            {

                return false;
            }
            if (indexofdot != -1)
            {
                string classname = input.Substring(0, indexofdot);  
                if (!_lieutenants.ContainsKey(classname))
                {
                    message = "No classname " + classname;
                    return false;
                }
                lt = _lieutenants[input.Substring(0, indexofdot)];
                
                methodname = input.Substring(indexofdot + 1);
            }
            else
            {
                methodname = input;
                if (_methodtolieutenant.ContainsKey(input))
                {
                    if (_methodtolieutenant[input].Count != 1)
                    {
                        string s = "";
                        foreach (Lieutenant l in _methodtolieutenant[input])
                            s += " " + l.CSObject.GetType().Name;
                        message = "Class options: " + s;
                        return false;
                    }
                    else
                        lt = _methodtolieutenant[input][0];
                }
                else
                {
                    message = input + " is unknown class or method";
                    return false;
                }
            }

            if (string.IsNullOrWhiteSpace(methodname) || lt == null)
            {
                message = "null class or methodname";
                return false;
            }
            return true;
        }
        #endregion
        #region Public functions
        [Help("Tries to issue the command. Returns true if successful.")] //assumptions: methodnames and classnames are intelligently made. If there are collisions in class and method, we assume class
        //also, classnames arent similar. We can't tell the difference between two classes MATH and Math
        //calling execute:
        //!math.sin 30
        //!sin 30
        public bool Execute(string inputstring, AdditionalParameters additionalparms, out object[] refvals)
        {
            
            refvals = new object[1];
            if (string.IsNullOrWhiteSpace(inputstring))
                return false;
            
            string[] words = _customsplit(inputstring);
            words[0] = words[0].ToLower();

            string methodname;
            Lieutenant lt;
            string message;
            if (!_dotnotation(words[0], out lt, out methodname, out message))
            {
                refvals[0] = message;
                return false;
            }

            if (_disallowedcommands.Contains(methodname))
            {
                refvals[0] = message;
                return false;
            }

            return lt.Execute(methodname, words.Skip(1).ToArray(), additionalparms, out refvals);
        }

              [Help("")]
        [ViewerPermitted]
        public string Classes()
        {
            string ret="";
            foreach(Lieutenant l in _lieutenants.Values)
            {
                ret += l.CSObject.GetType().ToString() + " ";

            }
            return ret;
        }

        [Help("Generates a readme and opens it")]
        public void Readme()
        {
            using (StreamWriter writer = new StreamWriter(DIRECTORY+"README.TXT"))
            {
                writer.WriteLine("== Hello UpThere! ==");
                writer.WriteLine("This project hasn't been updated to use Twitch API v5 (this currently uses v2!)");
                writer.WriteLine("As such, it doesn't actually connect to the Twitch API. Feel free to play with");
                writer.WriteLine("the following features:");
                writer.WriteLine("   -Any commands listed in the COMMANDS reference below, such as");
                writer.WriteLine("       !splat");
                writer.WriteLine("       !ping google.com 3");
                writer.WriteLine("       !roll 3d6");
                writer.WriteLine("       !asynchronous");
                writer.WriteLine("       !help roll");
                writer.WriteLine("   -Write your own commands by creating cs files in the");
                writer.WriteLine("    UserScripts/Autoload directory.");
                writer.WriteLine("   -Open notepad (or other application), and input the following commands");
                writer.WriteLine("       !play notepad");
                writer.WriteLine("       !type \"hello world\" ");
                writer.WriteLine("       !rightclick");
                writer.WriteLine(" ");
                
                StringBuilder sb = new StringBuilder();
                writer.WriteLine("==CREATING COMMANDS==");
                writer.WriteLine("Any command tagged as 'viewerpermitted' can be run by viewers");
                writer.WriteLine("For Virilay, to allow viewers access to this command");
                writer.WriteLine("     void Roll(string dieroll)");
                writer.WriteLine("Add the following code");
                writer.WriteLine("     [ViewerPermitted]");
                writer.WriteLine("     void Roll(string dieroll)");
                writer.WriteLine("If you wanted to add help text when users type !help, use the");
                writer.WriteLine("help attribute.");
                writer.WriteLine("");
                writer.WriteLine("Use Console.WriteLine or return to report text");
                writer.WriteLine("If you want to use an external framework, add the dlls to");
                writer.WriteLine("UserScripts/dll. Remember to also create a text file!");
                writer.WriteLine("");

                writer.WriteLine("==RULES REFERENCE==");
                foreach (Lieutenant lt in _lieutenants.Values)
                {
                    writer.WriteLine("=" + lt.CSObject.ToString().ToUpper() + "=");
                    foreach (string key in lt.Methods.Keys)
                    {
                        foreach (MethodInfo mi in lt.Methods[key])
                        {
                            sb.AppendFormat("{0} {1}(", mi.ReturnType.Name.ToLower(), mi.Name);
                            ParameterInfo[] pis = mi.GetParameters();
                            if (pis.Length > 0)
                            {

                                sb.AppendFormat("{0} {1}", pis[0].ParameterType.Name.ToLower(), pis[0].Name);
                                for (int i = 1; i < pis.Length; i++)
                                    sb.AppendFormat(", {0} {1}", pis[i].ParameterType.Name.ToLower(), pis[i].Name);
                            }
                            sb.AppendFormat(")");
                            writer.WriteLine(sb.ToString());
                            sb.Clear();
                            HelpAttribute ha = mi.GetCustomAttribute<HelpAttribute>();
                            if (ha != null) writer.WriteLine(ha.HelpText);
                            foreach (CustomAttributeData attr in mi.CustomAttributes)
                                writer.WriteLine("[" + attr.AttributeType.Name + "]");
                            writer.WriteLine();
                        }
                    }
                }

                writer.WriteLine("");
                writer.WriteLine("=TYPES=");
                writer.WriteLine("INTEGER");
                writer.WriteLine("An integer number. Fractions not allowed");
                writer.WriteLine("");
                writer.WriteLine("UINT");
                writer.WriteLine("An unsigned integer number. Negative numbers, and fractions are not allowed. 0 is permitted.");
                writer.WriteLine("");
                writer.WriteLine("DOUBLE, FLOAT, DECIMAL");
                writer.WriteLine("A number that decimals, such as -3.3, 3, or 2");
                writer.WriteLine("");
                writer.WriteLine("CHAR");
                writer.WriteLine("A single character, such as 'a', '1', or 1.");
                writer.WriteLine("");
                writer.WriteLine("STRING");
                writer.WriteLine("Many characters, such as 3d6+2");
                writer.WriteLine("If you want to include space, surround the words with \", such as:");
                writer.WriteLine("\t\"This is a string\"");
            }
            Process.Start("README.txt");
        }

        [Help("Generates the next unique filename. For example, NextFile(\"Logs\\log\",\".log\") might generate 'Logs\\log3.log'")]
        public static string NextFile(string prefix, string filetype)
        {
            int i = 1;
            string filename;
            while (File.Exists(filename = prefix + (i++) + filetype))
            { }
            return filename;
        }

        static DateTime _logwriternow;
        static string _logname = "";


        [Help("Writes text to a logfile. Returns the name of the file if we created a new file")]
        [ViewerPermittedAttribute]
        public static string Log(string text, string origin)
        {
            string ret = "";
            if (_logname == "")
            {
                _logname = NextFile("Logs\\", ".log");
                _logwriternow = DateTime.Now;
                ret = "Created " + _logname;
            }
            using (StreamWriter sw = new StreamWriter(_logname, true))
                sw.WriteLine((DateTime.Now - _logwriternow).ToString("hh':'mm':'ss") + "\t" + origin + "\t" + text.ToString());
            return ret;
        }


        [Help("Writes text to a logfile. Returns the name of the file if we created a new file")]
        [ViewerPermittedAttribute]
        [AdditionalParameters]
        public static string Log(string text, AdditionalParameters ap)
        {
            return Log(text, ap.Origin);
        }


        [Help("Opens the log")]
        public void Log()
        {
            if (_logname != "")
            {
                Process.Start(_logname);
            }
            else Console.WriteLine("No log");
        }

        [Help("Opens the working directory, where the exe is stored")]
        public void Location()
        {
            Process.Start(Directory.GetCurrentDirectory());
        }


        [Help("Loads a sourcefile. Returns true if successfully compiled. Otherwise, reports the reason in 'reason'")]
        public bool Compile(string filetitle)
        {
            string csfile = "UserScripts\\" + filetitle + ".cs";
            string reffile = "UserScripts\\dlls\\" + filetitle.Substring(filetitle.IndexOf("\\") + 1) + ".txt";
            string classname = filetitle.Substring(filetitle.LastIndexOf('\\') + 1);
            classname = char.ToUpper(classname[0]) + classname.Substring(1);

            if (!File.Exists(csfile))
            {

                Log(csfile + " does not exist","COMPILER");
                return false;
            }

            using (StreamReader sr = File.OpenText(csfile))
            {
                #region Compile sourcecode
                #region Compile settings
                string source = sr.ReadToEnd();
                bool usingforms = false;
                if (Regex.IsMatch(source, "\bForm\b"))
                {
                    usingforms = true;
                    StringBuilder sb = new StringBuilder();
                    sb.Append("[STAThread]static void Main(){");
                    sb.Append("Application.EnableVisualStyles();Application.SetCompatibleTextRenderingDefault(false);Application.Run(new ");
                    sb.Append(classname + "();}");
                    Console.WriteLine(sb.ToString());
                    source.Insert(source.IndexOf('{') + 1, sb.ToString());
                }

                Dictionary<string, string> providerOptions = new Dictionary<string, string>
                {
                    {"CompilerVersion", "v4.0"}
                };
                CSharpCodeProvider provider = new CSharpCodeProvider(providerOptions);

                CompilerParameters compilerParams = new CompilerParameters
                {
                    GenerateInMemory = true,
                    GenerateExecutable = false,
                };
                var assemblies = AppDomain.CurrentDomain
                                .GetAssemblies()
                                .Where(a => !a.IsDynamic)
                                .Select(a => a.Location);

                compilerParams.ReferencedAssemblies.Add("Viri.dll");
                if (File.Exists(reffile))
                {
                    string[] lines = File.ReadAllLines(reffile);
                    foreach (string line in lines)
                    {
                        try { compilerParams.ReferencedAssemblies.Add("UserScripts\\dlls\\" + line); }
                        catch
                        {
                            Log(reffile+" ponts to nonexistent dll components\\dlls\\"+line,"COMPILER");
                            return false;
                        }
                    }
                }
                compilerParams.ReferencedAssemblies.Add("system.dll");
                compilerParams.ReferencedAssemblies.Add("mscorlib.dll");
                compilerParams.ReferencedAssemblies.Add("system.xml.dll");
                compilerParams.ReferencedAssemblies.Add("system.data.dll");
                compilerParams.ReferencedAssemblies.Add("system.web.dll");
                compilerParams.ReferencedAssemblies.Add("system.core.dll");
                compilerParams.ReferencedAssemblies.Add("System.Windows.Forms.dll");
                compilerParams.ReferencedAssemblies.Add("System.Drawing.dll");

                if (usingforms)
                {
                    Console.WriteLine("Compiling " + filetitle + " as windows application");
                    compilerParams.CompilerOptions = "/optimize /target:winexe";
                    compilerParams.GenerateExecutable = true;
                    compilerParams.OutputAssembly = classname + ".exe";
                }//compilerParams.ReferencedAssemblies.Add("DbLib.dll");*/

                #endregion


                #region Show errors and quit if necessary
                CompilerResults results = provider.CompileAssemblyFromSource(compilerParams, source);
                if (results.Errors.Count != 0)
                {
                    Console.WriteLine(filetitle + ": Compiler errors");
                    Log(filetitle + " errors", "COMPILER");
                    foreach (CompilerError ce in results.Errors)
                    {
                        Log("Line" + ce.Line + ": " + ce.ErrorText, "COMPILER");
                    }
                    Log(); //opens log
                    return false;

                }
                #endregion


                #region Create the object
                //Supposing the file path is "UserScripts/Namespace/Foo.cs", the class name is Foo 
                if (!usingforms)
                {
                    object o = null;
                    try
                    {
                        o = results.CompiledAssembly.CreateInstance(classname, true);
                    }
                    catch (Exception e)
                    {
                        Log(e.ToString(), "COMPILER");
                        return false;
                    }
                    if (File.Exists(filetitle + ".ico") || filetitle.IndexOf("ame1") != -1)
                    {
                        /*Form fo = (Form)o;
                        fo.Icon = new Icon(filetitle + ".ico");
                        fo.Show();*/
                    }
                    if (o == null)
                    {
                        Log("The filename and the class name need to be the same. The first letter needs to be uppercased.\nCompile(" + filetitle + ").." + classname,"COMPILER");;
                        return false;
                    }

                    MethodInfo[] infos = o.GetType().GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
                #endregion
                #endregion
                    AddLieutenant(o);
                }
                else
                {
                    //  Process p = Process.Start(classname + ".exe");
                    //    _processes.Add(p);
                }
            }
            Console.WriteLine("Compiled " + classname);
            return true;
        }
        List<Process> _processes = new List<Process>();

        public void Dispose()
        {
            foreach (Process p in _processes)
            {
                p.Close();
            }
        }

        string[] _customsplit(string command)
        {
            List<string> ret = new List<string>();
            int index = 0;
            bool inquotes = false;
            StringBuilder currentword = new StringBuilder();
            while (index < command.Length)
            {
                switch (command[index])
                {
                    case '"': //special case: ""
                        
                        if (inquotes)
                        {
                            ret.Add(currentword.ToString());
                            currentword.Clear();
                        }
                        inquotes = !inquotes;
                        
                        break;
                    case ' ':
                        if (!inquotes && !string.IsNullOrWhiteSpace(currentword.ToString()))
                        {
                            ret.Add(currentword.ToString());
                            currentword.Clear();
                        }
                        else currentword.Append(command[index]);
                        break;
                    default:
                        //if (readingstring)
                        currentword.Append(command[index]);
                        //else
                        //    currentword.Append(command[index].ToString().ToLower());
                        break;
                }
                index++;
            }
            if (!string.IsNullOrEmpty(currentword.ToString()))
                ret.Add(currentword.ToString());

            return ret.ToArray();
        }

        [Help("Prevents plebians from using the function")]
        public void Allow(string command)
        {
            if (_disallowedcommands.Contains(command))
            {
                _disallowedcommands.Remove(command);
                Console.WriteLine(command + " allowed");
            }
            else
            {
                _disallowedcommands.Add(command);
                Console.WriteLine(command + " disallowed");
            }
        }


        public static bool CanConvert(string value, Type type)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(type);
            return converter.IsValid(value);
        }

        #endregion


        void IDisposable.Dispose()
        {
            throw new NotImplementedException();
        }

        //TEST SUITE                    
        /*public int Values(int a, ref int b, out int c)
        { 
            a = 100;
            b = 200;
            c = 300;
            return 400;
        }
        public string Values(string a, ref string b, out string c)
        {
            a = "Nonexistent";
            b = "Second+"+b;
            c = "third";
            return "First";
        }
        static void Main()
        {
            Executor _exec = new Executor();
            object[]retvalues;
            _exec.Execute("Values 1 2 3", new AdditionalParameters(), out retvalues);
            for(int i=0;i<retvalues.Length;i++)
            {
                Console.WriteLine(retvalues[i].GetType()+" "+retvalues[i]);
            }
            System.Threading.Thread.Sleep(10000);
        }*/
    }

    //Help attributes that describe the significance of functions and their parameters
    public class HelpAttribute : System.Attribute
    {
        public readonly string HelpText;
        public HelpAttribute(string helptext)
        {
            HelpText = helptext;
        }
    }



    [Help("Any function tagged as 'ViewerPermitted' can be run by plebians")]
    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class ViewerPermittedAttribute : System.Attribute
    {
    }
    public partial class AdditionalParameters
    {
        Dictionary<string, FieldInfo> _fields = new Dictionary<string, FieldInfo>();
        public AdditionalParameters()
        {
            FieldInfo[] fis = this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo fi in fis)
            {
                _fields.Add(fi.Name.ToLower(), fi);
            }
        }

        public T GetField<T>(string field)
        {
            return ((T)_fields[field.ToLower()].GetValue(this));

        }
    }
    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class AdditionalParametersAttribute : System.Attribute
    {

    }
    public partial class AdditionalParameters
    {
        public string Origin;

    }
}
