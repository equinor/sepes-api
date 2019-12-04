# Git workflow

(example)

Say I want to develop a feature. I will then...

1. Create a feature branch from branch `master` in local repo
   ```sh
   # Be sure that you got the latest changes (ie pull request) in local repo
   git checkout master
   git pull
   git checkout -b "#some-issue-id-and-name"
   ```
1. Loop until done
   1. Develop and test in feature branch
   1. Commit and push changes to remote repo
      ```sh
      git add .
      git commit -m "Some small change"
      git push
      ```
1. When development is done, create a pull request and run code review in github  

   If code review is OK 
   1. Squash & merge into branch `master`
   1. Delete feature branch in github
   1. Delete feature branch in local repo
      ```sh
      git checkout master
      git pull
      git branch -d "#some-issue-id-and-name"
      ```
   1. Done!
   
   If code review is _not_ OK
   1. Fix problems locally
   1. Commit and push changes to feature branch (pull request will be updated)
   1. Run code review
      - If OK then jump to "If code review is OK" list
      - If not OK then repeat this list
