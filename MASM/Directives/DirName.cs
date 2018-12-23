namespace Directives
{
    public sealed class DirNames
    {
        public static readonly DirNames Define   = new DirNames("#define");
        public static readonly DirNames Import   = new DirNames("#import");
        public static readonly DirNames Macros   = new DirNames("#macros");
        public static readonly DirNames IfNDef   = new DirNames("#ifndef");
        public static readonly DirNames IfDef    = new DirNames("#ifdef");
        public static readonly DirNames If       = new DirNames("#if");
        public static readonly DirNames Else     = new DirNames("#else");
        public static readonly DirNames Repeat   = new DirNames("#repeat");
        public static readonly DirNames ForEach  = new DirNames("#foreach");
        public static readonly DirNames EndIF    = new DirNames("#endif");
        public static readonly DirNames EndMacro = new DirNames("#endmacro");
        public static readonly DirNames EndFor   = new DirNames("#endfor");
        public static readonly DirNames EndCycle = new DirNames("#endcycle");

        private DirNames(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }
    }

}
