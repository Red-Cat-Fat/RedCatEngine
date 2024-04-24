#if !ODIN_INSPECTOR_3

    namespace Editor
    {
        public interface ISearchFilterable
        {
            bool IsMatch(string searchString);
        }
    }
    
#endif
