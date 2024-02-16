# MMD Blendshape Generator
非破壊MMD用シェイプキー生成ツール

## 概要
既に存在しているシェイプキーを組み合わせてMMD用のシェイプキーを生成できるツールです

シェイプキーで顔を編集すると既存のMMD用シェイプキーが破綻したりするのを何とかできます

## インストール

プロジェクト内の適当な場所で `git clone https://github.com/Gomorroth/MMDBlendshapeGenerator.git`

VCC対応はそのうちやるかも

## 使い方

- 顔のオブジェクト（`Body`であるはず）に`MMD Blendshape Generator`コンポーネントを追加

- MMD用シェイプキーの名前一覧が表示されるので、編集したいところを開き、組み合わせたいシェイプキーを追加していく
  - スライダーでシェイプキーの合成具合を調整できます。
    - ０以上の場合はそのままシェイプキーの０～１００に対応します
    - ０未満の場合、元あるシェイプキーを打ち消します（-1で完全に打ち消し）
