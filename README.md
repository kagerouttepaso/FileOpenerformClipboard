# FileOpenerformClipboard

[![Build Status](https://dev.azure.com/gothwasawasa/FileOpenerformClipboard/_apis/build/status/github_master?branchName=master)](https://dev.azure.com/gothwasawasa/FileOpenerformClipboard/_build/latest?definitionId=6&branchName=master)

## ダウンロード

[Release](https://github.com/kagerouttepaso/FileOpenerformClipboard/releases)からダウンロードできます。

## 使い方

1. リンクをコピーする
1. アプリケーションを起動する
1. 複数行に渡るリンク文字列を検索し、最もそれらしいリンクを規定のアプリケーションで開きます

### 開けるリンクの例

#### URLとか

``` text
http://google.com
https://google.com
```

#### サーバー上のfolderとか

``` text
\\server\folder
```

#### 複数行で記述されたファイルとか

``` text
ex1)
\\server\folder
filename.cpp

ex2)
\\server\folder\
filename.cpp
```

#### ファイルパスが装飾されていても大丈夫(Windowsのファイルパスに使えない記号に限る)

``` text
ex1)
  * \\server\folder\filename.cpp
```

→ Open `\\server\folder\filename.cpp`

``` text
ex2)
  < \\server\folder\
filename.cpp >
```

→ Open `\\server\folder\filename.cpp`

#### 前後に関係ない文字列があってもファイルが開ける

``` text
ex)
ファイルパスをお送りします。
\\Localserver\folder\sub folder\sub_fol
der2\longlongforlder

filename.cpp

以上よろしくお願いいたします。
```

-> Open `\\Localserver\folder\sub folder\sub_folder2\longlongforlder\filename.cpp`
