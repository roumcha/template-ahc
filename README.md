# 準備

- .NET SDK 6.0
- VSCode
- Code Runner
- 解答に使う言語のコンパイル環境

# 設定

- settings.jsonc を記入
- ahc-tool-temp/in に入力ケースを配置
- VSCode の設定で Code Runner の実行コマンドを設定
- 必要なら ahc-tool を改造

# メモ

- WSL の Ubuntu 上でバイナリを実行できない

`UseShellExecute = false` とか、絶対パスとかが怪しい。

けど、それを変えると標準入出力のリダイレクトができなかったり、FileInfo でパスを持てなかったりするのでこのまま Windows で使う。
