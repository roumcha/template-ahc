# template-ahc

AtCoder Heuristic Contest に使っている各言語のテンプレートと、並列テストツール。

個人用に作成したものであり、正確さや、すべてのコンテストで使用できる保証はありません。

また、利用するには CLI の操作や、VSCode の利用に慣れている必要があります。

## 対応 OS

- Windows 10 以上: 動きます
- Linux: 動きません　動くと思ったんだけど
- macOS: 不明です　持ってないので

## 対応言語

- C++, Rust, C#, F#
- Python
- その他、実行ファイルにコンパイルされる任意の言語\
  （一からフォルダや設定を構成する必要があります）

## 事前準備

- [.NET SDK 7.0](https://dotnet.microsoft.com/ja-jp/download/dotnet/7.0)
- [Visual Studio Code](https://code.visualstudio.com/download)
  - [Code Runner](https://marketplace.visualstudio.com/items?itemName=formulahendry.code-runner) 拡張機能を追加済み

このほか、使用する言語のコンパイル・実行環境も用意。例えば、

- C# / F# → [.NET Runtime 3.1](https://dotnet.microsoft.com/ja-jp/download/dotnet/3.1) をインストール
- Rust → Rustup で 1.42.0 ツールチェーンをインストール
- Python → Python (できれば 3.8) をインストール。\
  Python Launcher (`py.exe`) のデフォルトで実行されます。
- C++ → 使用するコンパイラをインストール

## 初期設定

### 1. このテンプレートリポジトリをもとに、今回のコンテスト用のリポジトリを作成

### 2. `ahc-tool-temp/in` の中にテストケースを配置

### 3. `settings.jsonc` を記入

- `SubmissionBinOrScript`\
  コンパイル後に実行ファイルができる場所を指定。Python の場合はスクリプトの場所で OK。

- `IsInteractiveProblem`\
  インタラクティブ問題の場合は `true` に変更する。

- `TestTool` / `EvaluationTool`\
  コンテスト中に公式から配布される、テストケースの実行や採点を行うプログラム。\
  適当な場所に設置して、その場所を書く。\
  インタラクティブ問題なら TestTool、そうでなければ EvaluationTool に書く。

- `TestToolArgs` / `EvaluationToolArgs`\
  公式採点ツールに与える引数を書く。\
  以下の引数を使うと、実行時にパスと置き換えられる。

  - $in（入力ファイルの場所）
  - $out（出力ファイルの場所）
  - $cmd（提出コードの実行コマンド）

- `ParallelDegree`\
  並列テストの同時実行数。\
  正確を期すなら CPU 使用率が 100% に張りつかない程度か、もっと減らしたほうがいいかも。

### 4. 使わない言語の `src-○○` フォルダは削除して OK

### 5. 使う言語の `src-○○` フォルダをルートディレクトリとして VSCode を起動

### 6. `src-○○/.vscode/settings.json` の編集

`code-runner.executorMapByGlob` にコマンドを設定して、以下が一度にできるようにする

- 提出コードをコンパイル
- その実行ファイルを、上記 2 で `SubmissionBinOrScript` に設定した場所に配置する
- このファイル (`README.md`) のある階層に `cd`
- `dotnet run --project ahc-tool -c Release` を実行

## 使用方法

提出用ソースファイルを書き終えたら、そのまま開いた状態で Ctrl + Alt + N を押す。

待つ。

結果が出力される。

各テストケースごとの詳細は、`ahc-tool-temp/out_yyyy-MM-dd/` に出力される。
