# JoyToVRC 

VRChatをJoy-Conでデスクトップモードで遊ぶとき Joy-Con のIMUの情報で手を動かばいいのでは？と思い作りました。<br />
すごく動くわけじゃないのでちょっとでも手が動いた方が嬉しい人向け。

仕組みとしては、SteamがJoy-Conの情報握っていますが、裏で別のソフトを動かしても同時に情報取れることを利用しOSCでアバターExParamに情報送信・アニメーションで動作です。

[バイナリは boothで配布しています。](https://booth.pm/ja/items/6080890)
(githubのほうが新しい場合があります)

## (JoyToVRCなしで)まず、Joy-ConでVRChat デスクトップモードで遊べるようにする
### ①VRChatをJoyCon で操作するために、まずPCに接続します。
　説明：[https://github.com/tbbsakura/VBTTools/blob/main/docs/JoyconConnect.md](https://github.com/tbbsakura/VBTTools/blob/main/docs/JoyconConnect.md)

### ②VRC起動してから、Steamの画面で入力の設定をします<br />
Setting0.jpg<br /><img src="docs/img_readme/Setting0.jpg" width="60%" /> <br />の1番のところのメニューから設定、コントローラーと進み次の画像のように設定します。<br />
Setting1.jpg<br /> <img src="docs/img_readme/Setting1.jpg" width="40%" /> <br />
<p>
さらにSetting0.jpg の2番のところのコントローラーアイコンをクリックします。
　Steam入力を有効にしてない場合は空白の多い画面に有効にするボタンがあるので有効にします。
</p>
<p>
現在のボタンのレイアウト、というところで VRChat-Official Profile for Switch Pro Controller の 公式レイアウト、を選びます。（右の▶入って推奨の中の一番下）
下のほうは背面ボタンを有効化して、右SLにRキーを割り当てます（パイメニュー出す用）
Rに設定するのは、コマンドを追加を押して上のほうのキーボードを選びRを選びます。
</p>　
<p>
設定後は<br /> Setting2.jpg<br /> <img src="docs/img_readme/Setting0.jpg" width="50%" /><br /> のようになります。
（開いてるところに機能追加してもかまいません。Shift+FNキーで一発表情ボタンとかにもできるはず）
</p>　
<p>
これでVRChatで遊べることを確認しておいてください。
</p>　

## JoyToVRC で 手を動かす方法
### ③アバターは TMARelay で手を動かせるアバターにしておきます。
  TMARelay はこちら [https://booth.pm/ja/items/5056237](https://booth.pm/ja/items/5056237) <br />
TMARelay_v0.2.1MA.ZIP というのを使います。

ModularAvatar があれば、TMARelay_MA_Both の prefab をアバター直下に入れるだけです。
詳しい説明はこちら [https://tbbsakura.fc2.net/blog-entry-19.html](https://tbbsakura.fc2.net/blog-entry-19.html) 

また、パイメニュー（さきほどの右SLで出せます）を出して、右スティックとトリガーで、
上のOptionに入り、さらに、OSCを選びます。Enabledを有効にしたあと、同じところの左 Reset Configをしておきます。

### ④JoyToVRCを起動して、Joy-Con Startを押し、しばし待ちます。
　赤と青の直方体が出てきたら、スティックで動かしてみます。<br/>
　動かない場合はアプリ終了してやりなおしてください。<br/>
　動いたらSendOSCにチェックを入れればアバターの手が動きます。<br/>
　標準操作はVRCのウィンドウを選択しないとできないので選択しておきます。<br/>

### ⑤キャリブレーション
コントローラーと直方体の向きがあってないときは、水平に持ち（右手なら、プラスが自分から遠い側の上面にあり、自分向きに赤外線の枠がある状態）ZR/ZLを使うと位置調整されます。<br/>
（ゲーム中にトリガー使うとこれが発動するので、都度水平で再調整してください。）

## ライセンス情報
MIT License. <br/>
Copyright (c) 2024 Sakura(さくら) / tbbsakura<br/>

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.

以下のMITライセンスのソフトを一部改変して使っています
[uOSC](https://github.com/hecomi/uOSC) [ライセンス情報](https://github.com/hecomi/uOSC/blob/master/LICENSE.md)
[Joyconlib](https://github.com/Looking-Glass/JoyconLib) [ライセンス情報](https://github.com/Looking-Glass/JoyconLib/blob/master/LICENSE)


## Build
当リポジトリのファイルを git clone した後に、uOSC は前述のサイトの unitypackage を入れてください。<br/>
(Joyconlibは改変してあるのが入ってるので、入れない（上書きしない）でください。)
<br/>
あとは シーン Assets/SakuraShop_tbb/JoyToVRC/JoyToVRC を開いて Build します。
