# Knowledgebase

## Using reStructuredText
See http://docutils.sourceforge.net/docs/user/rst/quickref.html

## Add File to Git History
Lets assume we would like to add a file after the first commit of the repo, which already has a comple of commits

    git log

Let's first checkout the commit before the new commit

    git checkout master

In order to match with the history if the repository, we might change the date of the additional commit to something in the past.

    export GIT_COMMITTER_DATE="Tue Jan 17 18:49:30 2017 +0100"
    export GIT_AUTHOR_DATE="Tue Jan 17 18:49:30 2017 +0100"

Let's add the the file and also add a commit message¨

    git add LICENSE
    git commit --date="Tue Jan 17 18:49:30 2017 +0100" -m "License added"

The new commit should show in the history. We need to rebase the development branch so that the development branch bases on the new commit instead of the commit before.

Checkout the develop branch first

    git checkout develop

Then rebase to the new commit, which might be master now. The `-commiter-date-is-author-data` keeps the commiterdate and author date in sync and does not introduce new timestamps

    git rebase master --committer-date-is-author-date

The istory is noew rewritten, but an commiter-author has been introduces since the commits have been rewritten. The following command resets all these fields to the corresponding author-values

    git filter-branch --commit-filter 'export GIT_COMMITTER_NAME="$GIT_AUTHOR_NAME"; export GIT_COMMITTER_EMAIL="$GIT_AUTHOR_EMAIL"; export GIT_COMMITTER_DATE="$GIT_AUTHOR_DATE"; git commit-tree "$@"'

Only thing left is to push now