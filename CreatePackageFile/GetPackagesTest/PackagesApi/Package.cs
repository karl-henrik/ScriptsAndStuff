namespace PackagesApi
{
    public class Package
    {
        public string package { get; set; }
    }

    public class Rootobject
    {
        public Package[] packages { get; set; }
    }
}