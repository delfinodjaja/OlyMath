namespace OlyMath.Models
{
    public class Material
    {
        public int MaterialId { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string ContentUrl { get; set; }
        public int ModuleId { get; set; }
        public virtual Module Module { get; set; }
    }
}
