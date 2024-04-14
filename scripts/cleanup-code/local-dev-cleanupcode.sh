#!/bin/bash

# Exit codes
SUCCESS=0
INVALID_ARGUMENT_ERROR=1
YOU_NEED_NO_CHANGES_BEFORE_RUN_CLEANUP_ERROR=3

# Default arguments' values
AUTO_COMMIT=yes

echo ""
echo "--- --- ---"
echo "Alright Cleanup Code Command-Line Tool"
echo "Default settings:"
echo "- auto commit re-formatted code (-a): '$AUTO_COMMIT'"
echo "--- --- ---"
echo ""

while getopts a: flag
do
    case "${flag}" in
        a) AUTO_COMMIT=${OPTARG};;
        *) echo ""
           echo "--- --- ---"
           echo "Invalid argument's flag is not handled"
           echo "--- --- ---"
           echo ""
           exit $INVALID_ARGUMENT_ERROR ;;
    esac
done

if [ "$AUTO_COMMIT" != "yes" ] && [ "$AUTO_COMMIT" != "no" ]
then
    echo ""
    echo "--- --- ---"
    echo "INVALID ARGUMENT OF '-a' equals '$AUTO_COMMIT'"
    echo "Set 'yes' or 'no' or omit to use default equals 'no'"
    echo "--- --- ---"
    echo ""
    exit $INVALID_ARGUMENT_ERROR
fi

UNSTAGED_CHANGES=$(git diff --name-only)
if [ -z "$UNSTAGED_CHANGES" ]
then
    echo ""
    echo "--- --- ---"
    echo "Right, there are no unstaged changes"
    echo "--- --- ---"
    echo ""
else
    echo ""
    echo "--- --- ---"
    echo "There are unstaged changes"
    echo "Commit them before run the script"
    echo "--- --- ---"
    echo ""

    git diff --name-only
    exit $YOU_NEED_NO_CHANGES_BEFORE_RUN_CLEANUP_ERROR
fi

STAGED_UNCOMMITTED=$(git diff --staged --name-only)
if [ -z "$STAGED_UNCOMMITTED" ]
then
    echo ""
    echo "--- --- ---"
    echo "Right, there is no any changes, repo is ready to cleanup"
    echo "--- --- ---"
    echo ""
else
    echo ""
    echo "--- --- ---"
    echo "There are staged, uncommitted changes"
    echo "Commit them before run the script"
    echo "--- --- ---"
    echo ""

    git diff --staged --name-only
    exit $YOU_NEED_NO_CHANGES_BEFORE_RUN_CLEANUP_ERROR
fi

echo ""
echo "--- --- ---"
echo "Restore dotnet tools (the JetBrains CleanupCode Tool)" 
echo "--- --- ---"
echo ""

dotnet tool restore
dotnet jb cleanupcode --version

echo ""
echo "--- --- ---"
echo "Let's get started, keep calm and wait, it may take few moments"
echo "--- --- ---"
echo ""

dotnet jb cleanupcode NTeoTestBuildeR.sln --verbosity=WARN --exclude=**.verified.json

REFORMATTED_FILES=$(git diff --name-only)

if [ -z "$REFORMATTED_FILES" ]
then
    echo ""
    echo "--- --- ---"
    echo "No files re-formatted, everything is clean, congratulation!"
    echo "--- --- ---"
    echo ""
    exit $SUCCESS
fi

if [ "$AUTO_COMMIT" = "no" ]
then
    echo ""
    echo "--- --- ---"
    echo "There is re-formatted code but it will not be auto committed"
    echo "--- --- ---"
    echo ""
    exit $SUCCESS
fi

echo ""
echo "--- --- ---"
echo "There are re-formatted files to be committed"
echo "--- --- ---"
echo ""

git diff --name-only

for FILE in "${REFORMATTED_FILES[@]}"
do
    git add ${FILE}
done

echo ""
echo "--- --- ---"
echo "Staged files to be committed"
echo "--- --- ---"
echo ""

git diff --staged --name-only

echo ""
echo "--- --- ---"
echo "Create commit"
echo "--- --- ---"
echo ""

git commit -m "Cleanup: re-format code by JetBrains CleanupCode Tool"

echo ""
echo "--- --- ---"
echo "Commit created"
echo "--- --- ---"
echo ""

git status

echo ""
echo "--- --- ---"
echo "All re-formatted code has been committed with success"
echo "--- --- ---"
echo ""
exit $SUCCESS