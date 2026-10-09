public readonly struct SpawnEntry
{
    public readonly EnemyDefinition Definition;
    public readonly SpawnPoint Point;

    public SpawnEntry(EnemyDefinition definition, SpawnPoint point)
    {
        Definition = definition;
        Point = point;
    }
}
