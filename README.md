FileOpenerformClipboard
=======================

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
```

## 注意
ファイルパスの解析アルゴリズムがヘボいので、
あまり大きな行数をコピーすると加速度的にメモリを食います。  
15行行くらいまでなら60MB程度ですみますが、
18行目で550MB、20行では1.3GBほどメモリを食い出します。
