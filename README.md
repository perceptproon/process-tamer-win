# process-tamer-win

Windows 11 + Intel Core 第12世代だと一部のアプリで不具合が起こる。202409時点だとLINE Desktopで画像を開くとなぜか全CPUをしばらく食いつぶす / 古いゲームなどで想定外にE-Core動作となり
期待通りの性能が出ないなど。
Windowsの自動割り当てには大変幻滅したので「定期的にプロセス一覧を取得し、CPU Affinityやプロセス優先度を制御する」常駐プログラムを作ることにした。
似たようなプロダクトはいくつかあるが、そんなに高性能なものは求めていないので自分用にシンプルなものを作る。

# Requirement

* Visual Studio 2022 Community

# Usage

同梱の"config.default.yaml"を"config.yaml"にコピーし、コメントを参考に編集する。
ビルドしてexeファイルを生成し、"config.yaml"を同じディレクトリに配置する。
設定出来たらタスクスケジューラなどでWindows起動時に実行させる。


# Note

あくまで個人用プロダクトなので、UIなどは最低限。破壊的変更も行うかも。

# Author

perceptproon (perceptproon@gmail.com)

# License

"process-tamer-win" is under [MIT license](https://en.wikipedia.org/wiki/MIT_License).

わざわざprivate repoにするほどでもないのでpublic repo配置としていますが、あくまで自分専用のプロダクトですので、機能追加要望などは一切受け付けておりません。
万が一このプロダクトを使いたい方がいた場合はご自由にforkしてください。
