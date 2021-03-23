namespace RegulSaveCleaner.Structures
{
    public class CleanerResult
    {
        public double OldSize { get; set; }

        public double NewSize { get; set; }
        
        public string Save { get; set; }

        public CleanerResult(double oldsize, double newsize, double totalSecond, string save)
        {
            OldSize = oldsize;
            NewSize = newsize;
            TotalSecond = totalSecond;
            Save = save;
        }

        public double TotalSecond { get; set; }
    }
}
