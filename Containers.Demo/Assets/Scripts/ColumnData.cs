﻿using Sources;

public struct ColumnData
{
    public ContainerPlatform Platform;
    public ContainerColumn Column;

    public int MinLoadingDepth;

    public int LowerSum;
    public int UpperSum;
    public int MaxLowerDepth;
}