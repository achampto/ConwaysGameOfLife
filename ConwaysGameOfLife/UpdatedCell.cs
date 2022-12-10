namespace ConwaysGameOfLife
{
    /// <summary>
    /// Contains the information about a cell that should be updated.
    /// </summary>
    /// <param name="xPosition">The x position of the cell.</param>
    /// <param name="yPosition">The y position of the cell.</param>
    /// <param name="isAlive">Whether the cell is alive after being updated.</param>
    internal record UpdatedCell(long xPosition, long yPosition, bool isAlive);
}
