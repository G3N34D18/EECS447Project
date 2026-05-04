namespace ArenaSync.Web.Dtos
{
    public class EntityTableColumn<TItem>
    {
        public string Header { get; set; } = string.Empty;
        public Func<TItem, string> Value { get; set; } = _ => string.Empty;
    }
}
