﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

namespace BootstrapBlazor.Components;

/// <summary>
/// 大小
/// </summary>
public struct MapSize
{
    /// <summary>
    /// 构造
    /// </summary>
    public MapSize()
    {
        Width = null;
        Height = null;
    }

    /// <summary>
    /// 构造
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public MapSize(double width, double height)
    {
        Width = width;
        Height = height;
    }

    /// <summary>
    /// 宽度
    /// </summary>
    public double? Width { get; set; }

    /// <summary>
    /// 高度
    /// </summary>
    public double? Height { get; set; }
}
