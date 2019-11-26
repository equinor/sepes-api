# Developer docs

## How to build & run

(example)

`docker-compose up --build`

## How to deploy

(example)

1. Merge branch `master` into branch `release` in local repo
   1. Fix any merge problems
1. Push to remote repo
1. Done!

```sh
# Be sure to have latest updates in master
git checkout master
git pull
# Be sure to have latest updates in release
git checkout release
git pull
# Merge and push
git merge master
git push
```

## How to test

[Testing](./testing/)
