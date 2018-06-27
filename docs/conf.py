import os
import sys
import sphinx_rtd_theme
extensions = []
templates_path = ['_templates']

source_suffix = '.rst'
master_doc = 'index'

project = u'Letstalk Business Account'
copyright = u'2018, Letstalk'
author = u'Letstalk'
version = '1.3'
release = '1.3'

exclude_patterns = []
pygments_style = 'sphinx'

html_theme = 'sphinx_rtd_theme'
html_static_path = ['_static']
html_theme_path = [sphinx_rtd_theme.get_html_theme_path()]

def setup(app):
    app.add_stylesheet("theme_overrides.css")

htmlhelp_basename = 'LetstalkBusinessAccountdoc'
html_context = {
    "display_gitlab": True, # Integrate Gitlab
    "gitlab_host": "gitlab.nexcel.vn",
    "gitlab_user": "rome/alpha", # Username
    "gitlab_repo": "letstalk-bot", # Repo name
    "gitlab_version": "master", # Version
    "conf_py_path": "/docs/", # Path in the checkout to the docs root
}

latex_elements = {
    # The paper size ('letterpaper' or 'a4paper').
    #
    # 'papersize': 'letterpaper',

    # The font size ('10pt', '11pt' or '12pt').
    #
    # 'pointsize': '10pt',

    # Additional stuff for the LaTeX preamble.
    #
    # 'preamble': '',

    # Latex figure (float) alignment
    #
    # 'figure_align': 'htbp',
}

latex_documents = [
    (master_doc, 'LetstalkBusinessAccount.tex', u'Letstalk Business Account Documentation',
     u'Letstalk', 'manual'),
]
latex_show_urls = 'footnote'

man_pages = [
    (master_doc, 'letstalkbusinessaccount', u'Letstalk Business Account Documentation',
     [author], 1)
]

texinfo_documents = [
    (master_doc, 'LetstalkBusinessAccount', u'Letstalk Business Account Documentation',
     author, 'LetstalkBusinessAccount', 'One line description of project.',
     'Miscellaneous'),
]
texinfo_show_urls = 'footnote'

epub_title = project
epub_author = author
epub_publisher = author
epub_copyright = copyright
epub_exclude_files = ['search.html']
epub_show_urls = 'footnote'