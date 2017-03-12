---
layout: post
title:  "Improved Multiple Monitor Support"
date:   2012-04-01 12:00:00
categories: release
---
License Status had basic support for multiple monitors in that the window would be restored to whichever monitor it was last used on. The problem was that it would restore to a monitor even if the monitor was disconnected. I've updated it to ensure that the window will be visible regardless of monitor configuration or resolution changes.
