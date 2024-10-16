# Local Device Finder
[English]((./README.md)) | 日本語

Local Device Finderは、ローカルネットワーク上のデバイスを検出するためのC#ライブラリです。UDPブロードキャストとマルチキャストを使用してディスカバリーメッセージを送信し、デバイスからの応答を待ち受けます。
## 特徴
- ローカルネットワーク内のデバイスを検出します。
- EditorとRuntimeの両方で動作します。
- ポート設定を適切に行えば、1台のPCで送受信のテストが可能です。
- 応答はカスタマイズ可能です。サンプルではデバイス名のみを応答として返していますが、任意の情報をレスポンスに追加することが可能です。
- UDPブロードキャストおよび、ネットワーク負荷が低いマルチキャストに対応しています。
- 検出したデバイスに個別にデータを送信することが可能です。

## インストール

OpenUPM を使用して`Local Device Finder`パッケージをインストールするには、プロジェクトのルートフォルダで次のコマンドを実行します：

```
openupm add com.afjk.com.afjk.local-device-finder
```

または

Unityパッケージマネージャを使用して`Local Device Finder`パッケージをインストールするには、次の手順に従います：

1.	UnityEditorでWindow -> Package Manager
2.	左上の + ボタンをクリック
3.	Add package from git URL....
4.	次のURLを貼り付けてEnterを押す

```
https://github.com/afjk/LocalDeviceFinder.git?path=/Packages/com.afjk.local-device-finder#v0.0.1
```

## Samples
Package ManagerからLocal Device Finderのサンプルをインポートしてください。

### Editor Sample

LocalDeviceFinderEditorとLocalDeviceResponderEditorは、Unityエディタ内でデバイスの検出と応答をテストするためのサンプルです。 

これらのエディタウィンドウを開くには、Unityエディタで`Tools > Local Device Finder > Finder`または`Tools > Local Device Finder > Responder`をクリックします。  

### Runtime Sample

`LocalDeviceFinderClient`と`LocalDeviceResponderClient`は、実行時にデバイスの検出と応答を行うためのサンプルです。

これらのクラスは、Unityのゲームオブジェクトにアタッチして使用します。

サンプルシーン`Assets/Samples/Scenes/SampleScene.unity`には、これらのクラスを使用したサンプルが含まれています。
