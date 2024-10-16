# Local Device Finder
[English](./README.md) | 日本語

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
openupm add com.afjk.local-device-finder
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

![image](https://github.com/user-attachments/assets/23d8f72b-4bbe-434e-ad5f-e55686d13c76)

### Editor Sample
![image](https://github.com/user-attachments/assets/4ba8838b-e592-4285-b16c-59fcbe550ca9)

LocalDeviceFinderEditorとLocalDeviceResponderEditorは、Unityエディタ内でデバイスの検出と応答をテストするためのサンプルです。 

これらのエディタウィンドウを開くには、Unityエディタで`Tools > Local Device Finder > Finder`または`Tools > Local Device Finder > Responder`をクリックします。  

### Runtime Sample
<img src="https://github.com/user-attachments/assets/0d1a92e9-802c-4720-be93-2dd930b2b4fa" width="50%">

`LocalDeviceFinderClient`と`LocalDeviceResponderClient`は、実行時にデバイスの検出と応答を行うためのサンプルです。

これらのクラスは、Unityのゲームオブジェクトにアタッチして使用します。

サンプルシーン`Assets/Samples/Local Device Finder/0.0.1/Sample/Demo/Demo.unity`には、これらのクラスを使用したサンプルが含まれています。
