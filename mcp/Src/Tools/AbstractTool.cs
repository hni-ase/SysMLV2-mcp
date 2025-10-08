namespace Tools
{
    public abstract class AbstractTool
    {
        protected AbstractTool(string name, string description)
        {
            this.Name = name;
            this.Description = description;
        }

        public string Name { get; }
        public string Description { get; }

        public abstract void HandleOperation(object? value);
    }
}