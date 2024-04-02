public interface IDebuff
{
    string Identifier { get; }
    void Apply(Target target);
    void Tick(Target target);
    void Refresh();
    void Remove(Target target);
}
