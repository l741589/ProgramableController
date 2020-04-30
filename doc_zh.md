# 简介

# API

## 全局API

### global
全局对象

### 跟原版javascript保持一致的对象和函数
```javascript
Date, Number, String, Function，Boolean, JSON, RegExp, Object, Array, Math, Error, 
NaN, Infinity, 
eval, parseInt, parseFloat, isNaN, isFinite, decodeURI, decodeURIComponent, encodeURI, encodeURIComponent, escape, unescape
```
### setTimeout(callback, delay [,...args])
* callback &lt;Function&gt; 当定时器到点时调用的函数。
* delay &lt;number&gt; 调用 callback 之前等待的毫秒数。如果不传将被视为传 0，callback将会在下一帧调用。
* ...args &lt;any&gt; 当调用 callback 时传入的可选参数。
* 返回: &lt;number&gt; 用于 clearTimeout()。
安排在 delay 毫秒之后执行一次性的 callback。
当 delay 为空或  小于等于 0 时， callback会在下一帧立即调用。
非整数的 delay 会被截断为整数。
如果 callback 不是函数，则抛出 TypeError。

```javascript
setTimeout(function(){
//invoke next second
},1000);

setTimeout(function(){
//invoke next frame
});
```

### setInterval(callback, delay [,...args])
* callback &lt;Function&gt; 当定时器到点时调用的函数。
* delay &lt;number&gt; 调用 callback 之前等待的毫秒数。如果不传或者传递0，callback将会从下一帧开始每一帧都调用。
* ...args &lt;any&gt; 当调用 callback 时传入的可选参数。
* 返回: &lt;number&gt; 用于 clearInterval()。

安排每隔 delay 毫秒重复执行 callback。
当 delay 为空或  小于等于 0 时， callback从下一帧开始，每一帧都被调用，这在制作某些每帧检测事件时会非常有用。
非整数的 delay 会被截断为整数。
如果 callback 不是函数，则抛出 TypeError。

```javascript
setInterval(function(){
//invoke every second
},1000);

setInterval(function(){
//invoke every frame
});
```

### clearTimeout(timmer)
取消由 setTimeout() 创建的定时器
* timmer &lt;number&gt; setTimeout() 返回的计时器

```javascript
var timer = setTimeout(function(){},1000);
clearTimeout(timer);
```

### clearInterval(timmer)
取消由 setInterval() 创建的定时器
* timmer &lt;number&gt; setInterval() 返回的计时器

```javascript
var timer = setInterval(function(){},1000);
clearInterval(timer);
```

### console
控制台对象，控制在besiege游戏的控制台输出
#### log(...args)
打印参数到控制台，并加上换行符。 可以传入多个参数，会被空格隔开输出

#### dir(data)
结构化打印对象
* 如果对象是null，undefined, string, number, boolean 基本类型 效果等同console.log(...)
    ```javascript 
    console.dir(1);
    console.dir(true);
    console.dir('1');
    console.dir(null);
    console.dir(undefined);
    ```
* 如果对象是javascript object, 将会将其json序列化后输出
    ```javascript 
	console.log({a:1,b:1});
    console.log([1,2,3]);
    ```
* 如果对象是一个native object, 将会打印出其字段, 属性, 方法的信息
    ```javascript 
	console.log(console);
    console.log(process);
    ```

#### clear()
清空控制台
```javascript
console.clear();
```
#### command(cmd)
执行控制台命令
```javascript
console.command('clear');
```
### process
进程对象，控制当前cpu运行上下文信息
#### pid &lt;number&gt;
当前脚本所运行在的cpu组件的id

#### title &lt;string&gt;
固定值 `'string'`

### machine 
机械控制相关的对象

#### keyDown(key) keyUp(key) keyPress(key[,timeout]) 
模拟按钮事件，其中`keyDown`模拟按钮按下，`keyUp`模拟按钮抬起，`keyPress`模拟按钮按下后抬起
* key &lt;string&gt;|&lt;number&gt; 用于模拟的按键，具体的值见附录。如果填写的是数字，可以模拟一些不在按键表里面的按键
* timeout &lt;number&gt; 对于keyPress，可以设定按下时长，以毫秒为单位。
```javascript
machine.keyDown('a');
machine.keyUp('DownArrow');
machine.keyPress(1001);
```
#### subscribeKeyDown(key,callback) subscribeKeyUp(key,callback) subscribeKeyPress(key,callback)
监听按钮事件，其中`subscribeKeyDown` 在按钮按下时每帧持续触发，`subscribeKeyUp` 在按钮抬起时触发，`subscribeKeyPress` 在按钮按下的时触发一次，同一个key可以有多个监听
* key &lt;string&gt;|&lt;number&gt; 用于监听的按键事件，具体的值见[附录](#按键表)。如果填写的是数字，可以模拟一些不在按键表里面的按键
* callback &lt;function(key)&gt; 相应事件的回调，会回传key的数字值
```javascript
machine.subscribeKeyDown('a',function(key){/*do sth. here*/});
machine.subscribeKeyUp('DownArrow',function(key){/*do sth. here*/});
machine.subscribeKeyPress(1001,function(key){/*do sth. here*/});
```
#### unsubscribeKeyDown(key) unsubscribeKeyUp(key) unsubscribeKeyPress(key)
取消监听对应的按钮事件，会取消该监听该key的所有事件。
* key &lt;string&gt;|&lt;number&gt; 用于监听的按键事件，具体的值见[附录](#按键表)。如果填写的是数字，可以模拟一些不在按键表里面的按键

#### getBlocksByName(name)
根据name查找零件
* name &lt;string&gt; name, 模块的名字, 等价于返回Block的name属性，大小写不敏感
* 返回 &lt;[Block](##零件-API)[]&gt; 查找到的零件
```javascript
var wheels = machine.getBlocksByName('wheel');
var steerings = machine.getBlocksByName('SteeringBlock');
```
#### getBlockByName(name)
getBlocksByName的单数版本，只返回找到的第一个

#### getBlockByGuid(guid)
根据guid查找零件，由于guid的唯一性，该api不存在复数版本
* guid &lt;string&gt; guid, 模块的名字, 等价于返回Block的guid属性，大小写不敏感
* 返回 &lt;[Block](##零件-API)&gt; 查找到的零件
```javascript
var block = machine.getBlockByGuid('c9f2ffe4-b213-4d29-ac8d-ca1fea51c0c5');
```

#### getBlocksByClassName(className)
根据零件的className查找零件，className在每个零件的配置面板都可以填写。
* className &lt;string&gt; name, 模块的名字, 等价于返回Block的className属性，大小写不敏感。每个零件可能有多个className, 这里只用匹配其中一个即可。
* 返回 &lt;[Block](##零件-API)[]&gt; 查找到的零件
```javascript
var forwardBlocks = machine.getBlocksByName('forward-group');
var backwardBlocks = machine.getBlocksByName('backward-group');
```

#### getBlockByClassName(className) 
getBlocksByClassName 的单数版本，只返回找到的第一个

### storage
整个机械共用的存储模块，可以在多个cpu之间共享的kv数据存储
#### setItem(key,value)
设置缓存值，直接访问同名成员也可以达到同样效果。
* key &lt;string&gt;，表示存储的key
* value 除了undefined之外，类型不限，表示要存储的值，如果传入undefined，该函数将不进行任何操作。

```javascript
storage.setItem('myKey1',1);
storage.myKey2 = {a:1,b:2};
storage['my-key-3'] = [1,2,3,4];
```
#### getItem(key)
* key &lt;string&gt;，表示读取的key，直接访问同名成员也可以达到同样的效果
* 返回 该key对应的值，如果不存在该key，返回undefined
```javascript
var value1 = storage.getItem('myKey1');
var value2 = storage.myKey2;
var value3 = storage['my-key-3'];
```

#### removeItem(key)
删除key
* key 需要删除的key
```javascript
storage.removeItem('key');
```

#### clear()
清空所有值
```javascript
storage.clear();
```

#### length &lt;number&gt;
该存储中key的数量

#### key(index)
获取第index个key
* index &lt;number&gt; key的序号, 从0开始
```javascript
var firstKey = storage.key(0);
var lastKey = storage.key(storage.length-1);
```

#### subscribe([key,]callback)
监听指定key的变化, 当指定key的值发生变化的时候，触发callback
* key &lt;string&gt; 存储的key, 省略该字段时，将监听所有key的变化
* callback &lt;function(key,newValue,oldValue)&gt; 值发生变化时的响应函数
```javascript
storage.subscribe('myKey1',function(key,newValue,OldValue){});
storage.subscribe(function(key,newValue,OldValue){});
```

#### unsubscribe([key])
取消监听key上的所有监听
* key &lt;string&gt; 存储的key, 省略该字段时，将取消整个storage所有的监听。

### message
可以在多个cpu之间发送消息的模块

#### broadcast(body)
向所有cpu(包括自己)发送消息
* body 类型不限，发送的消息内容
```javascript
message.broadcast('this is a message');
message.broadcast({a:1,b:2});
```

#### send(cpuId,body)
向指定cpu发送消息
* cpuId &lt;number&gt; 接收者的id
* body 类型不限，发送的消息内容
```javascript
var targetCpuId = machine.getBlockByClassName('targetCpuClassName').id;
message.send(targetCpuId, 'this is a message');
```

#### subscribe(callback)
为本cpu注册一个接受消息的函数，没有调用该函数的cpu将无法接收到别人发来的消息
* callback &lt;function(messageContent)&gt; 接收消息的函数
    * messageContent.id &lt;number&gt; 消息的唯一id
    * messageContent.timestamp &lt;Date&gt; 消息的发送时间
    * messageContent.sender &lt;number&gt; 发送者id
    * messageContent.target &lt;number&gt; 接收者id，广播消息这个值为-1
    * messageContent.body, 类型由发送者决定，消息内容
```javascript
message.subscribe(function(msg){
    console.log(msg.id);
    console.log(msg.timestamp)
    console.log(msg.sender);
    console.log(msg.target);
    console.log(msg.body);
});
```
#### unsubscribe(callback)
本cpu取消消息订阅

## 零件 API
Block为零件对象，使用machine.getXXX(...) 获得的零件就是该对象，之后就需要使用Block API对零件进行操作
### 零件通用
所有零件都拥有的属性和函数
#### guid &lt;string&gt;
零件的全局唯一id
#### name &lt;string&gt;
零件的名称，同样类型的零件拥有同样的name， 比如所有的动力轮都叫做wheel
#### className &lt;string[]&gt;
玩家在编辑零件属性时填写的值，多个值用空格或者半角逗号分隔
#### print()
在控制台打印该零件的信息，包括名字，guid，和属性列表，可以作为编写代码时的参考
#### getAttribute(key)
获取零件的属性
* key &lt;string&gt; 属性的名称，直接访问同名成员也有同样的效果
* 返回 &lt;MapperType&gt; 零件属性对象，具体操作可以参考 [零件属性 API](#零件属性-API)
```
var wheel = machine.getBlockByName('wheel');
var forwardKey = wheel.getAttribute('forward');
var backwardKey = wheel.backward;
```

### 可激活零件通用
继承：[零件通用](#零件通用)

可激活模块是只包含激活和非激活两种状态的零件
包含 [速度计](#速度计) [高度计](#高度计) [方向计](#方向计) [侦测器](#侦测器) [计时器](#计时器) [逻辑门](#逻辑门) [钩爪](#钩爪) [活塞](#活塞) 

#### isActive &lt;boolean&gt;
该零件是否处于激活状态

#### subcribe(callback)
监听该零件的激活状态变化
* callback &lt;function(boolean)&gt; 回调，会传入一个变化后的激活状态

#### unsubscribe()
取消监听该零件的激活状态变化

### 速度计
继承: [可激活零件通用](#可激活零件通用) [零件通用](#零件通用)
#### sqrSpeed &lt;number&gt;
速度的平方，比speed属性性能高
#### speed &lt;number&gt;
速度
#### sqrValue &lt;number&gt;
等价于sqrSpeed
#### value &lt;number&gt;
等价于speed

### 高度计
继承: [可激活零件通用](#可激活零件通用) [零件通用](#零件通用)
#### height &lt;number&gt;
高度
#### value &lt;number&gt;
等价于height

### 方向计
继承: [可激活零件通用](#可激活零件通用) [零件通用](#零件通用)
#### angle &lt;number&gt;
角度
#### value &lt;number&gt;
等价于angle

### 计时器
继承: [可激活零件通用](#可激活零件通用) [零件通用](#零件通用)

无额外属性


### 逻辑门
继承: [可激活零件通用](#可激活零件通用) [零件通用](#零件通用)

#### a &lt;boolean&gt;
第一个输入
#### b &lt;boolean&gt;
第二个输入
#### operator &lt;string&gt;
操作符有 NOT, AND, OR, NOR, NAND, XOR, XNOR 这几种取值
#### value &lt;boolean&gt;
等价于 isActive

### 侦测器
继承: [可激活零件通用](#可激活零件通用) [零件通用](#零件通用)
#### count &lt;number&gt;
侦测到的物体数量
#### forward &lt;number[3]&gt;
表示侦测器朝向的向量
#### value &lt;object[]&gt;
侦测到的物体列表
* value[].name &lt;string&gt; 物体的名字，如果是零件则是零件的Name，否则生物为Creature，其他为Entity
* value[].forward &lt;number[3]&gt; 表示侦测器朝向的向量
* value[].target &lt;number[3]&gt; 从侦测器到目标的向量
* value[].sqrDistance &lt;number&gt; 从侦测器到该物体距离的平方，性能比distance好
* value[].distance &lt;number&gt;从侦测器到该物体距离
* value[].targetForward &lt;number[3]&gt; 从侦测器到目标的向量在侦测器朝向上的投影向量
* value[].sqrDistanceForward &lt;number&gt; 从侦测器到该物体在侦测器朝向上距离的平方，性能比distanceForward好
* value[].distanceForward &lt;number&gt; 从侦测器到该物体在侦测器朝向上的距离
* value[].isBlock &lt;boolean&gt; 该物体是不是零件
* value[].isProjectile &lt;boolean&gt; 该物体是不是投射物
* value[].isCreature &lt;boolean&gt; 该物体是不是生物
* value[].isDead &lt;boolean&gt; 该物体是不是已经死亡，只对生物有效
* value[].isStatic &lt;boolean&gt; 该物体是否静态物体
* value[].blockName &lt;string&gt; 该物体的名称，只对零件有效
* value[].canBreak &lt;boolean&gt; 该物体是否可破坏


### 钩爪
继承: [可激活零件通用](#可激活零件通用) [零件通用](#零件通用)

无额外属性


### 活塞
继承: [可激活零件通用](#可激活零件通用) [零件通用](#零件通用)

无额外属性

## 零件属性 API

零件的属性可以通过 [Block.GetAttribute()](#getAttribute(key)) 来获取

每个零件有什么属性，对配置界面上的名字，可以通过 [Block.print()](#print()) 在控制台打印出来。

属性的字段除非特殊注明只读，都是允许读写的。

### MKey 按键
#### value &lt;string[]&gt; 当前配置的按键列表
#### isUp &lt;boolean&gt; 当前按钮是抬起状态, 只读
#### isPress &lt;boolean&gt; 当前帧发生了按钮按下事件, 只读
#### isDown &lt;boolean&gt; 当前按钮是按下状态, 只读
#### emultating &lt;boolean&gt; 当前按钮是模拟按下状态, 只读
#### keyDown() keyUp() keyPress([timeout])
模拟 按下/抬起/点击 这个按键。只对当前属性所属的零件生效。不需要传入按键值，系统会自动使用配置的值。
对于keyPress函数，可以传入毫秒数，表示按下后多长时间再抬起
这几个函数可以用来精确控制特定零件的行为。
```javascript
var wheel = machine.getBlockByName('wheel');
wheel.forward.keyDown();
setTimeout(function() {
  wheel.forward.keyUp();
  wheel.backward.keyDown();
},10000)
```

### MColourSlider 颜色选择器
#### value &lt;string&gt; html格式的颜色，例如 `#ffffff` 这种格式的字符串

### MLimits 角度限制器
#### isActive &lt;boolean&gt; 限制器是否生效
#### max &lt;number&gt; 角度上限
#### min &lt;number&gt; 角度下限

### MMenu 单选框
#### items &lt;string[]&gt; 选项列表
#### value &lt;number&gt; 选中选项的序号
#### selection &lt;string&gt; 选中选项的字符串值

### MSlider 滑条
#### value &lt;number&gt; 滑条的值

### MToggle 开关按钮
#### isActive &lt;boolean&gt; 是否激活
#### value &lt;boolean&gt; 等价于isActive

### MValue 数值
#### value &lt;number&gt; 数值

### MCustom 自定义属性，由模组自定义的属性。
#### value 类型和含义均由所模组件定义。


# 附录
# 按键表
字符串大小写不敏感
|字符串表示|数字值|
|-|-|
|None|0|
|Backspace|8|
|Tab|9|
|Clear|12|
|Return|13|
|Pause|19|
|Escape|27|
|Space|32|
|Exclaim|33|
|DoubleQuote|34|
|Hash|35|
|Dollar|36|
|Ampersand|38|
|Quote|39|
|LeftParen|40|
|RightParen|41|
|Asterisk|42|
|Plus|43|
|Comma|44|
|Minus|45|
|Period|46|
|Slash|47|
|Alpha0|48|
|Alpha1|49|
|Alpha2|50|
|Alpha3|51|
|Alpha4|52|
|Alpha5|53|
|Alpha6|54|
|Alpha7|55|
|Alpha8|56|
|Alpha9|57|
|Colon|58|
|Semicolon|59|
|Less|60|
|Equals|61|
|Greater|62|
|Question|63|
|At|64|
|LeftBracket|91|
|Backslash|92|
|RightBracket|93|
|Caret|94|
|Underscore|95|
|BackQuote|96|
|A|97|
|B|98|
|C|99|
|D|100|
|E|101|
|F|102|
|G|103|
|H|104|
|I|105|
|J|106|
|K|107|
|L|108|
|M|109|
|N|110|
|O|111|
|P|112|
|Q|113|
|R|114|
|S|115|
|T|116|
|U|117|
|V|118|
|W|119|
|X|120|
|Y|121|
|Z|122|
|Delete|127|
|Keypad0|256|
|Keypad1|257|
|Keypad2|258|
|Keypad3|259|
|Keypad4|260|
|Keypad5|261|
|Keypad6|262|
|Keypad7|263|
|Keypad8|264|
|Keypad9|265|
|KeypadPeriod|266|
|KeypadDivide|267|
|KeypadMultiply|268|
|KeypadMinus|269|
|KeypadPlus|270|
|KeypadEnter|271|
|KeypadEquals|272|
|UpArrow|273|
|DownArrow|274|
|RightArrow|275|
|LeftArrow|276|
|Insert|277|
|Home|278|
|End|279|
|PageUp|280|
|PageDown|281|
|F1|282|
|F2|283|
|F3|284|
|F4|285|
|F5|286|
|F6|287|
|F7|288|
|F8|289|
|F9|290|
|F10|291|
|F11|292|
|F12|293|
|F13|294|
|F14|295|
|F15|296|
|Numlock|300|
|CapsLock|301|
|ScrollLock|302|
|RightShift|303|
|LeftShift|304|
|RightControl|305|
|LeftControl|306|
|RightAlt|307|
|LeftAlt|308|
|RightCommand|309|
|RightApple|309|
|LeftCommand|310|
|LeftApple|310|
|LeftWindows|311|
|RightWindows|312|
|AltGr|313|
|Help|315|
|Print|316|
|SysReq|317|
|Break|318|
|Menu|319|
|Mouse0|323|
|Mouse1|324|
|Mouse2|325|
|Mouse3|326|
|Mouse4|327|
|Mouse5|328|
|Mouse6|329|
|JoystickButton0|330|
|JoystickButton1|331|
|JoystickButton2|332|
|JoystickButton3|333|
|JoystickButton4|334|
|JoystickButton5|335|
|JoystickButton6|336|
|JoystickButton7|337|
|JoystickButton8|338|
|JoystickButton9|339|
|JoystickButton10|340|
|JoystickButton11|341|
|JoystickButton12|342|
|JoystickButton13|343|
|JoystickButton14|344|
|JoystickButton15|345|
|JoystickButton16|346|
|JoystickButton17|347|
|JoystickButton18|348|
|JoystickButton19|349|
|Joystick1Button0|350|
|Joystick1Button1|351|
|Joystick1Button2|352|
|Joystick1Button3|353|
|Joystick1Button4|354|
|Joystick1Button5|355|
|Joystick1Button6|356|
|Joystick1Button7|357|
|Joystick1Button8|358|
|Joystick1Button9|359|
|Joystick1Button10|360|
|Joystick1Button11|361|
|Joystick1Button12|362|
|Joystick1Button13|363|
|Joystick1Button14|364|
|Joystick1Button15|365|
|Joystick1Button16|366|
|Joystick1Button17|367|
|Joystick1Button18|368|
|Joystick1Button19|369|
|Joystick2Button0|370|
|Joystick2Button1|371|
|Joystick2Button2|372|
|Joystick2Button3|373|
|Joystick2Button4|374|
|Joystick2Button5|375|
|Joystick2Button6|376|
|Joystick2Button7|377|
|Joystick2Button8|378|
|Joystick2Button9|379|
|Joystick2Button10|380|
|Joystick2Button11|381|
|Joystick2Button12|382|
|Joystick2Button13|383|
|Joystick2Button14|384|
|Joystick2Button15|385|
|Joystick2Button16|386|
|Joystick2Button17|387|
|Joystick2Button18|388|
|Joystick2Button19|389|
|Joystick3Button0|390|
|Joystick3Button1|391|
|Joystick3Button2|392|
|Joystick3Button3|393|
|Joystick3Button4|394|
|Joystick3Button5|395|
|Joystick3Button6|396|
|Joystick3Button7|397|
|Joystick3Button8|398|
|Joystick3Button9|399|
|Joystick3Button10|400|
|Joystick3Button11|401|
|Joystick3Button12|402|
|Joystick3Button13|403|
|Joystick3Button14|404|
|Joystick3Button15|405|
|Joystick3Button16|406|
|Joystick3Button17|407|
|Joystick3Button18|408|
|Joystick3Button19|409|
|Joystick4Button0|410|
|Joystick4Button1|411|
|Joystick4Button2|412|
|Joystick4Button3|413|
|Joystick4Button4|414|
|Joystick4Button5|415|
|Joystick4Button6|416|
|Joystick4Button7|417|
|Joystick4Button8|418|
|Joystick4Button9|419|
|Joystick4Button10|420|
|Joystick4Button11|421|
|Joystick4Button12|422|
|Joystick4Button13|423|
|Joystick4Button14|424|
|Joystick4Button15|425|
|Joystick4Button16|426|
|Joystick4Button17|427|
|Joystick4Button18|428|
|Joystick4Button19|429|
|Joystick5Button0|430|
|Joystick5Button1|431|
|Joystick5Button2|432|
|Joystick5Button3|433|
|Joystick5Button4|434|
|Joystick5Button5|435|
|Joystick5Button6|436|
|Joystick5Button7|437|
|Joystick5Button8|438|
|Joystick5Button9|439|
|Joystick5Button10|440|
|Joystick5Button11|441|
|Joystick5Button12|442|
|Joystick5Button13|443|
|Joystick5Button14|444|
|Joystick5Button15|445|
|Joystick5Button16|446|
|Joystick5Button17|447|
|Joystick5Button18|448|
|Joystick5Button19|449|
|Joystick6Button0|450|
|Joystick6Button1|451|
|Joystick6Button2|452|
|Joystick6Button3|453|
|Joystick6Button4|454|
|Joystick6Button5|455|
|Joystick6Button6|456|
|Joystick6Button7|457|
|Joystick6Button8|458|
|Joystick6Button9|459|
|Joystick6Button10|460|
|Joystick6Button11|461|
|Joystick6Button12|462|
|Joystick6Button13|463|
|Joystick6Button14|464|
|Joystick6Button15|465|
|Joystick6Button16|466|
|Joystick6Button17|467|
|Joystick6Button18|468|
|Joystick6Button19|469|
|Joystick7Button0|470|
|Joystick7Button1|471|
|Joystick7Button2|472|
|Joystick7Button3|473|
|Joystick7Button4|474|
|Joystick7Button5|475|
|Joystick7Button6|476|
|Joystick7Button7|477|
|Joystick7Button8|478|
|Joystick7Button9|479|
|Joystick7Button10|480|
|Joystick7Button11|481|
|Joystick7Button12|482|
|Joystick7Button13|483|
|Joystick7Button14|484|
|Joystick7Button15|485|
|Joystick7Button16|486|
|Joystick7Button17|487|
|Joystick7Button18|488|
|Joystick7Button19|489|
|Joystick8Button0|490|
|Joystick8Button1|491|
|Joystick8Button2|492|
|Joystick8Button3|493|
|Joystick8Button4|494|
|Joystick8Button5|495|
|Joystick8Button6|496|
|Joystick8Button7|497|
|Joystick8Button8|498|
|Joystick8Button9|499|
|Joystick8Button10|500|
|Joystick8Button11|501|
|Joystick8Button12|502|
|Joystick8Button13|503|
|Joystick8Button14|504|
|Joystick8Button15|505|
|Joystick8Button16|506|
|Joystick8Button17|507|
|Joystick8Button18|508|
|Joystick8Button19|509|