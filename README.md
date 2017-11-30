FileOpenerformClipboard
=======================
[![Build status](https://ci.appveyor.com/api/projects/status/j2nm6tb7cd02gbeq?svg=true)](https://ci.appveyor.com/project/kagerouttepaso/fileopenerformclipboard)

## ダウンロード
[Release](https://github.com/kagerouttepaso/FileOpenerformClipboard/releases)からダウンロードできます。

## 使い方

1. リンクをコピーする
2. アプリケーションを起動する
3. 複数行に渡るリンク文字列を検索し、最もそれらしいリンクを規定のアプリケーションで開きます

### 開けるリンクの例

**URLとか**

```
http://google.com
https://google.com
```

**サーバー上のfolderとか**

```
\\server\folder
```

**複数行で記述されたファイルとか**

```
ex1)
\\server\folder
filename.cpp

ex2)
\\server\folder\
filename.cpp
```

**ファイルパスが装飾されていても大丈夫(Windowsのファイルパスに使えない記号に限る)**

```
ex1)
  * \\server\folder\filename.cpp 
```
→ Open `\\server\folder\filename.cpp`

```
ex2)
  < \\server\folder\
filename.cpp >
```
→ Open `\\server\folder\filename.cpp`


**前後に関係ない文字列があってもファイルが開ける**

```
ex)
ファイルパスをお送りします。
\\Localserver\folder\sub folder\sub_fol
der2\longlongforlder

filename.cpp

以上よろしくお願いいたします。
```

-> Open `\\Localserver\folder\sub folder\sub_folder2\longlongforlder\filename.cpp`
