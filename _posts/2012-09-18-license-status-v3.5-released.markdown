---
layout: post
title:  "License Status v3.5 Released"
date:   2012-09-18 12:00:00
categories: release
---
This update provides support for multiple checkouts by the same user.

For some situations a user can check out more than one license from the same host and display. In the lmstat report this is shown as:

`user11 comp11 comp11 (v2.0) (SERVER1/27001 209), start Sun 9/16 8:21, 2 licenses`

License Status will now read the number of licenses checked out and display it in the new 'Checked out' column. By default this column is not displayed, but it can be turned on anytime.

This change also corrects the issue where the feature information did not report the same number of licenses being used as the lmstat report did. Previously I assumed that the number of users would equal the number of licenses in use, which is not correct when the user can check out more than one license.
