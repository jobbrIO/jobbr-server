# Tooling
## .NET Projects
You'll need to install the following tools in order to compile the different projects

* Microsoft .NET Framework 4.6.2 Developer Pack and Language Packs from [Microsoft Download Center](https://www.microsoft.com/en-us/download/confirmation.aspx?id=53321)
* Microsoft Visual Studio 2015 from https://www.visualstudio.com/downloads/

## Documentation
The documentation is build by the readthedocs.org-Service. If you need to develop the documentation locally, install the following tools

* Phyton
* sphinx-1.6.2
* recommonmark-0.4.0

Build the documentation with `make clean&make html` and open the index in your browser.

**Autobuild**
We added a autobuild option the the make.bat but you need to install shpinx-autostart in order to work. Instructions: [GaretJax/sphinx-autobuild](https://github.com/GaretJax/sphinx-autobuild)

When installed, you can just issue `make livehtml` and point your browser to [http://localhost:8000](http://localhost:8000)

### Installation Guide for Windows
1. Download and install [Phyton >= 3.5.2 from the official download location](https://www.python.org/downloads/). 

    > **Add Phyton to PATH**: Make sure you check the Option to add Phyton to the PATH or you'll nee to patch the `PATH` Environment variable manually.

    Check your installation by executing `py` in the console. You should see something like this and you'll be able to exit by pressing `Ctrl-Z` and hit enter. 

    ```
    C:\users\michael>py
    Python 3.6.1 (v3.6.1:69c0db5, Mar 21 2017, 18:41:36) [MSC v.1900 64 bit (AMD64)] on win32
    Type "help", "copyright", "credits" or "license" for more information.
    ```
    > **New Console**: Keep in mind that environment variables (such as the `PATH`) will not change unless you start a new process with a console. That also applies to editors that host a console on their own.

2. Check PIP (The Python Package manager)
    
    The Pagage manager should be installed, just check with the command `pip --version`:

    ```
    C:\Users\michael>pip --version
    pip 9.0.1 from c:\users\michael\appdata\local\programs\python\python36\lib\site-packages (python 3.6)
    ```
    If there is no such command
    * Make sure you have restarted you console
    * Install pip by the command `py get-pip.py`

3. Install Sphinx

    Sphinx is the document rendering engine that is also used to generate jobbr.readthedocs.io. The following command will install Sphinx and all other related packages in your Phyton installation.

    ```
    C:\Users\michael>pip install sphinx
    Collecting sphinx
    Downloading Sphinx-1.6.2-py2.py3-none-any.whl (1.9MB)
        100% |████████████████████████████████| 1.9MB 618kB/s
    Collecting snowballstemmer>=1.1 (from sphinx)

    ...

    Successfully installed Jinja2-2.9.6 MarkupSafe-1.0 Pygments-2.2.0 alabaster-0.7.10 babel-2.4.0 certifi-2017.4.17 
    chardet-3.0.4 colorama-0.3.9 docutils-0.13.1 idna-2.5 imagesize-0.7.1 pytz-2017.2 requests-2.17.3 six-1.10.0 
    snowballstemmer-1.2.1 sphinx-1.6.2 sphinxcontrib-websupport-1.0.1 urllib3-1.21.1
    ```

4. Install CommonMark extension

    This extension is required to parse Markdown-files. Install it with `pip`.

    ```
    C:\Users\michael>pip install recommonmark
    ...
    Successfully installed commonmark-0.5.4 recommonmark-0.4.0
    ```

5. Install the official ReadTheDocsTheme (`sphinx_rtd_theme `)

    The official ReadTheDocs Theme can also be unstalled locally in order to test the build.

    ```
    C:\Users\michael>pip install sphinx_rtd_theme
    ...
    Successfully installed sphinx-rtd-theme-0.2.4
    ```    
6. Auto-reload

   There is a very useful extension that allow to host the documentation locally and refresh the browser on changes. 

   ```
    C:\Users\michael>pip install sphinx-autobuild
    ...
    Successfully installed sphinx-autobuild
    ```    
7. Further information

    * Installing Sphinx by the official spinx documentation on [http://www.sphinx-doc.org/en/stable/install.html](http://www.sphinx-doc.org/en/stable/install.html)
    * Adding markdown support to sphinx from [http://blog.readthedocs.com/adding-markdown-support/](http://blog.readthedocs.com/adding-markdown-support/)
    * Live reload extension from [https://github.com/GaretJax/sphinx-autobuild](https://github.com/GaretJax/sphinx-autobuild)