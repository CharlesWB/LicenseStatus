---
layout: post
title:  "License Status Update"
date:   2009-08-30 12:00:00
categories: release
---
Part of my work involves checking the status of FlexLM/FlexNet licenses. Usually I would use LMTools or the lmstat command line to get the status. While these get the job done I wanted something a little more usable. So I made my own program, License Status, and decided I'd make it available to anyone that wants it.

License Status reads the output of lmstat and presents it in a sortable table. Multiple licenses can be displayed. For those parts of lmstat that are not shown (such as redundant server information) the original lmstat report can be shown.

License Status requires .NET Framework 3.5 Service Pack 1.

##### Software Design

The other reason I wanted to create this program was to try out WPF.

After the initial learning curve, WPF's data binding and customizable controls made it easy to create the UI. But I did encounter two issues with License Status.

First, and I'm not sure if this is noticeable to everybody, is the [blurry font](https://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=380919&wa=wsignin1.0) display in WPF. Even though I ignored this in License Status I would be concerned about making a production program that users would be reading from frequently.

The second, and more significant, issue is as much my fault as the design of WPF. If you have several hundred features in a license there is a noticeable pause in the UI as the display is updated. If you have a thousand or more this pause becomes very noticeable.

This is a fault in my code because I have not optimized the list handling. To keep things simple I chose not resolve this in this version.

The other aspect of this issue is that it is how WPF's ListView is designed. For those who know WPF you know there is a VirtualizingStackPanel which basically improves the performance of a large list. The problem in License Status is that I've enabled grouping which [disables virtualization](http://web.archive.org/web/20111216064531/http://bea.stollnitz.com/blog/?p=338).

Possibly when .NET Framework 4.0 is released I'll reexamine these issues.

In case it would be useful to anyone else I've implemented a class library, LicenseManager.dll. This is where the actual processing of the lmstat output is done. This only requires .NET Framework 2.0 to make it more accessible.
