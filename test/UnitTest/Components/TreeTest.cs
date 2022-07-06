﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

namespace UnitTest.Components;

public class TreeTest : BootstrapBlazorTestBase
{
    [Fact]
    public void Items_Ok()
    {
        var cut = Context.RenderComponent<Tree>();
        cut.DoesNotContain("tree-root");

        // 由于 Items 为空不生成 TreeItem 显示 loading
        cut.Contains("table-loading");
        cut.DoesNotContain("li");

        cut.SetParametersAndRender(pb => pb.Add(a => a.ShowSkeleton, true));
        cut.Contains("skeleton tree");

        // 设置 Items
        cut.SetParametersAndRender(pb =>
        {
            pb.Add(a => a.Items, new List<TreeItem>()
            {
                new TreeItem() { Text = "Test1" }
            });
        });
        cut.Contains("li");
    }

    [Fact]
    public async Task OnClick_Checkbox_Ok()
    {
        var tcs = new TaskCompletionSource<bool>();
        bool itemChecked = false;
        var cut = Context.RenderComponent<Tree>(pb =>
        {
            pb.Add(a => a.IsAccordion, true);
            pb.Add(a => a.ShowCheckbox, true);
            pb.Add(a => a.OnTreeItemChecked, items =>
            {
                itemChecked = items.FirstOrDefault()?.Checked ?? false;
                tcs.SetResult(true);
                return Task.CompletedTask;
            });
            pb.Add(a => a.Items, new List<TreeItem>()
            {
                new TreeItem()
                {
                    Text = "Test1",
                    Items = new List<TreeItem>()
                    {
                        new TreeItem()
                        {
                            Text = "Test11",
                        }
                    }
                }
            });
        });

        // 测试点击选中
        await cut.InvokeAsync(() => cut.Find(".tree-node").Click());
        await tcs.Task;
        Assert.True(itemChecked);

        // 测试取消选中
        tcs = new TaskCompletionSource<bool>();
        await cut.InvokeAsync(() => cut.Find(".tree-node").Click());
        await tcs.Task;
        Assert.False(itemChecked);
    }

    [Fact]
    public async Task OnClick_Ok()
    {
        var clicked = false;
        var expanded = false;
        var cut = Context.RenderComponent<Tree>(pb =>
        {
            pb.Add(a => a.IsAccordion, true);
            pb.Add(a => a.ShowRadio, true);
            pb.Add(a => a.ClickToggleNode, true);
            pb.Add(a => a.OnTreeItemClick, item =>
            {
                clicked = true;
                return Task.CompletedTask;
            });
            pb.Add(a => a.OnExpandNode, item =>
            {
                expanded = true;
                return Task.CompletedTask;
            });
            pb.Add(a => a.Items, new List<TreeItem>()
            {
                new TreeItem()
                {
                    Text = "Test1",
                    Items = new List<TreeItem>()
                    {
                        new TreeItem()
                        {
                            Text = "Test11",
                        }
                    }
                },
                new TreeItem()
                {
                    Text = "Test2",
                    IsCollapsed = false,
                    Items = new List<TreeItem>()
                    {
                        new TreeItem()
                        {
                            Text = "Test21",
                        }
                    }
                }
            });
        });

        await cut.InvokeAsync(() => cut.Find(".tree-node").Click());
        Assert.True(clicked);
        Assert.True(expanded);
    }

    [Fact]
    public void OnStateChanged_Ok()
    {
        List<TreeItem>? checkedLists = null;
        var cut = Context.RenderComponent<Tree>(pb =>
        {
            pb.Add(a => a.ShowCheckbox, true);
            pb.Add(a => a.OnTreeItemChecked, items =>
            {
                checkedLists = items;
                return Task.CompletedTask;
            });
            pb.Add(a => a.Items, new List<TreeItem>()
            {
                new TreeItem()
                {
                    Text = "Test1",
                    Icon = "fa fa-fa",
                    CssClass = "Test-Class",
                    Items = new List<TreeItem>()
                    {
                        new()
                        {
                            Text = "Test1-1",
                            Items = new List<TreeItem>()
                            {
                                new()
                                {
                                    Text = "Test1-1-1"
                                }
                            }
                        }
                    }
                }
            });
        });

        cut.InvokeAsync(() => cut.Find("[type=\"checkbox\"]").Click());
        cut.DoesNotContain("fa fa-fa");
        cut.Contains("Test-Class");

        cut.SetParametersAndRender(pb =>
        {
            pb.Add(a => a.ShowIcon, true);
        });
        cut.Contains("fa fa-fa");
    }

    [Fact]
    public void Template_Ok()
    {
        var item = new TreeItem()
        {
            Text = "Test1",
            Key = "TestKey",
            Tag = "TestTag",
            Template = builder => builder.AddContent(0, "Test-Template")
        };
        var cut = Context.RenderComponent<Tree>(pb =>
        {
            pb.Add(a => a.Items, new List<TreeItem>()
            {
                item
            });
        });
        cut.Contains("Test-Template");
        Assert.Equal("TestKey", item.Key);
        Assert.Equal("TestTag", item.Tag);
    }

    [Fact]
    public async Task OnExpandRowAsync_Ok()
    {
        var expanded = false;
        var cut = Context.RenderComponent<Tree>(pb =>
        {
            pb.Add(a => a.OnExpandNode, item =>
            {
                expanded = true;
                return Task.CompletedTask;
            });
            pb.Add(a => a.Items, new List<TreeItem>()
            {
                new TreeItem()
                {
                    Text = "Test1",
                    Items = new List<TreeItem>()
                    {
                        new TreeItem()
                        {
                            Text = "Test11",
                            HasChildNode = true,
                            ShowLoading = true
                        }
                    }
                }
            });
        });

        await cut.InvokeAsync(() => cut.Find(".fa-caret-right").Click());
        Assert.True(expanded);
    }

    [Fact]
    public void GetAllSubItems_Ok()
    {
        var items = new List<TreeItem>()
        {
            new TreeItem() { Text = "Test1", Id = "01" },
            new TreeItem() { Text = "Test2", Id = "02", ParentId = "01" },
            new TreeItem() { Text = "Test3", Id = "03", ParentId = "02" },
        };

        var data = items.CascadingTree();
        Assert.Single(data);

        var subs = data.First().GetAllSubItems();
        Assert.Equal(2, subs.Count());
    }

    [Fact]
    public async Task ShowRadio_Ok()
    {
        List<TreeItem>? checkedLists = null;
        var cut = Context.RenderComponent<Tree>(pb =>
        {
            pb.Add(a => a.ShowRadio, true);
            pb.Add(a => a.OnTreeItemChecked, items =>
            {
                checkedLists = items;
                return Task.CompletedTask;
            });
            pb.Add(a => a.Items, new List<TreeItem>()
            {
                new() { Text = "Test1", Icon = "fa fa-fa" },
                new() { Text = "Test2", Icon = "fa fa-fa" }
            });
        });
        cut.Find("[type=\"radio\"]").Click();
        Assert.Single(checkedLists);
        Assert.Equal("Test1", checkedLists![0].Text);

        var radio = cut.FindAll("[type=\"radio\"]")[1];
        await cut.InvokeAsync(() => radio.Click());
        Assert.Equal("Test2", checkedLists![0].Text);

        cut.SetParametersAndRender(pb =>
        {
            pb.Add(a => a.ShowSkeleton, false);
        });
    }

    [Fact]
    public async Task Drag_Ok()
    {
        var cut = Context.RenderComponent<Tree>(pb =>
        {
            pb.Add(a => a.CanDrag, true);
            pb.Add(a => a.Items, new List<TreeItem>()
            {
                new() { Text = "Test5", Icon = "fa fa-fa"},
                new() { Text = "Test1", Icon = "fa fa-fa", Items = new List<TreeItem>()
                {
                    new() { Text = "Test3", Icon = "fa fa-fa"},
                    new() { Text = "Test4", Icon = "fa fa-fa"},
                }, IsCollapsed = false},
                new() { Text = "Test2", Icon = "fa fa-fa", CanDrag = false, Items = new List<TreeItem>()
                {
                    new() { Text = "Test6", Icon = "fa fa-fa"},
                    new() { Text = "Test7", Icon = "fa fa-fa"},
                    new() { Text = "Test8", Icon = "fa fa-fa"},
                },}
            });
        });
        var content = cut.FindAll(".tree-content")[1];
        await cut.InvokeAsync(() => content.DragStart());
        var test2 = cut.FindAll(".tree-content")[0];
        await cut.InvokeAsync(() => test2.DragEnter());
        await cut.InvokeAsync(() => test2.DragLeave());
        var space = cut.FindAll(".tree-space");
        await cut.InvokeAsync(() => space[0].DragEnter());
        await cut.InvokeAsync(() => space[0].DragLeave());
        space = cut.FindAll(".tree-space");
        await cut.InvokeAsync(() => space[1].DragEnter());
        await cut.InvokeAsync(() => space[1].DragLeave());
        space = cut.FindAll(".tree-space");
        await cut.InvokeAsync(() => space[2].DragEnter());
        await cut.InvokeAsync(() => space[2].DragLeave());
        await cut.InvokeAsync(() => content.DragEnd());

        content = cut.FindAll(".tree-content")[1];
        await cut.InvokeAsync(() => content.DragStart());
        test2 = cut.FindAll(".tree-content")[0];
        await cut.InvokeAsync(() => test2.DragEnter());
        await cut.InvokeAsync(() => content.Drop());

        var tree = cut.Find(".tree");
        await cut.InvokeAsync(() => tree.DragOver());
        await cut.InvokeAsync(() => tree.DragEnter());

        content = cut.FindAll(".tree-content")[3];
        await cut.InvokeAsync(() => content.Drop());

        content = cut.FindAll(".tree-content")[5];
        await cut.InvokeAsync(() => content.DragStart());
        space = cut.FindAll(".tree-space");
        await cut.InvokeAsync(() => space[14].DragEnter());
        await cut.InvokeAsync(() => content.Drop());
    }

}
