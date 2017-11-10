FileOpenerformClipboard
=======================
[![Build status](https://ci.appveyor.com/api/projects/status/j2nm6tb7cd02gbeq?svg=true)](https://ci.appveyor.com/project/kagerouttepaso/fileopenerformclipboard)

## ダウンロード
[Release](https://github.com/kagerouttepaso/FileOpenerformClipboard/releases)からダウンロードできます。

## 使い方

1. リンクをコピーする
2. アプリケーションを起動する

### 開けるリンクの例

URLとか

```
http://google.com
https://google.com
```

サーバー上のfolderとか

```
\\server\folder
```

複数行で記述されたファイルとか

```
ex1)
\\server\folder
filename.cpp

ex2)
\\server\folder\
filename.cpp
```

前後に関係ない文字列があってもファイルが開ける

```
invalid text
\\server\folder\
subfolder\subfolder2
filename.cpp
invalied string

-> open \\server\folder\subfolder\subfolder2\filename.cpp
```
