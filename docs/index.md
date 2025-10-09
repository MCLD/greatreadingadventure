---
layout: splash
permalink: /
hidden: true
header:
  overlay_color: "#000000"
  overlay_filter: 0.6
  overlay_image: /assets/images/forest@2x.jpg
  actions:
    - label: "<span class='fas fa-fw fa-laptop-code'></span> See the demo"
      url: "http://demo.greatreadingadventure.com/"
excerpt: >
  The Great Reading Adventure is free open-source software designed to manage online library reading programs.<br />
  <small><a href="https://github.com/MCLD/greatreadingadventure/releases/latest">Latest release v4.6.0</a></small>
---

The GRA is free to use, modify, and share. Source code is available on [GitHub](https://github.com/MCLD/greatreadingadventure). Active development is managed by the [Maricopa County Library District](https://mcldaz.org/). Questions? Join the [discussions](https://github.com/MCLD/greatreadingadventure/discussions)!

{% include feature_row %}

**Latest post**

{% assign posts = site.posts %}
{% assign entries_layout = page.entries_layout | default: 'list' %}

<div class="entries-{{ entries_layout }}" style="padding-left: 2rem; padding-right: 2rem;">
  {% for post in posts limit:1 %}
    {% include archive-single.html type=entries_layout %}
  {% endfor %}
</div>

[More posts](posts/)

---

_The Great Reading Adventure was developed by the [Maricopa County Library District](https://mcldaz.org/) with support by the [Arizona State Library, Archives and Public Records](https://www.azlibrary.gov/), a division of the Secretary of State, with federal funds from the [Institute of Museum and Library Services](https://www.imls.gov/)._
