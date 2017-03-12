---
layout: post
title:  "License Status v3.6 Released"
date:   2012-10-23 12:00:00
categories: release
---
This release improves the parsing of users that have spaces in their name or display.

To do this some rules were added that attempt to determine the user's host or display. These rules are relatively simple, so there will be situations where the user's information will not parse correctly. In those cases any spaces will be assumed to be part of the user's name.
