namespace HhhPrefabManagement
{
    /// <summary>
    /// Enum which decides what data type the <see cref="PrefabType"/> is constrained by.
    /// Basically controls how many different kinds of spawnable prefabs there may be.
    /// </summary>
    public enum PrefabTypeMode
    {
        /// <summary>
        /// Up to 255 (<see cref="byte.MaxValue"/>) different prefabs.
        /// </summary>
        Byte,

        /// <summary>
        /// Up to 32,767 (<see cref="short.MaxValue"/>) different prefabs.
        /// </summary>
        Short,

        /// <summary>
        /// Up to 2,147,483,647 (<see cref="int.MaxValue"/>) different prefabs.
        /// </summary>
        Int
    }
}