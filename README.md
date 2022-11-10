# 環境

- .NET SDK 7.0
- VSCode
  - Code Runner 拡張
- 解答に使う言語のコンパイル・実行環境

# 初期設定

- settings.jsonc を記入
- ahc-tool-temp/in に入力ケースを配置
- VSCode の設定で Code Runner の実行コマンドを設定
- 必要なら ahc-tool を改造
- 要らん言語のフォルダは削除

# メモ

- WSL の Ubuntu 上でバイナリを実行できない

`UseShellExecute = false` とか、絶対パスとかが怪しい。

けど、それを変えると標準入出力のリダイレクトができなかったり、FileInfo でパスを持てなかったりするのでこのまま Windows で使う。
