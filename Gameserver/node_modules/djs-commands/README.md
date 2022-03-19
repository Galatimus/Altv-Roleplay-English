# djs-commands
> An NPM Package to make creating new Discord.JS bots efficiently

[![NPM Version][npm-image]][npm-url]
[![Downloads Stats][npm-downloads]][npm-url]

Discordjs-commands is a NPM module that adds in a command handler and helpful functions


## Installation

```sh
npm install djs-commands --save
```

## Setup guide
*This is not a full Discord.JS bot tutorial. Please check out [TheSourceCode](https://www.youtube.com/channel/UCNXt2MrZaqfIBknamqwzeXA) for that.*

1 - To start using the Command Handler after installation, we'll first need require djs-commands and create a new Command Handler with the proper folder name and prefixes.

```js
const { CommandHandler } = require("djs-commands")
const CH = new CommandHandler({
    folder: __dirname + '/commands/',
    prefix: ['?', '??', 'tsc?']
  });
```

2 - Inside of the message event, we're going to do a little parsing and checking if they ran an available command or not.

```js
bot.on("message", (message) => {
    if(message.channel.type === 'dm') return;
    if(message.author.type === 'bot') return;
    let args = message.content.split(" ");
    let command = args[0];
    let cmd = CH.getCommand(command);
    if(!cmd) return;

    try{
        cmd.run(bot,message,args)
    }catch(e){
        console.log(e)
    }

});
```

3 - And of course we're going to need a command file. So inside of your bot folder, create a folder called commands. I'm going to create a file called
test.js and put the following code inside of it.

```js
module.exports = class test {
    constructor(){
            this.name = 'test',
            this.alias = ['t'],
            this.usage = '?test'
    }

    async run(bot, message, args) {
        await message.delete();
        message.reply(this.name + " worked!")
    }
}
```

4 - And that's it! You have a working command handler now for all the commands you could want!

## Release History

* 1.0.0
    * ADD: Readme, CommandHandler, and basic stuff I will need.
* 1.1.0
    * UPDATE: Changed system to work as module
* 1.1.3
    * UPDATE: Fixing file loader system
* 1.2.0
    * FIX: Fixed the loading system, added usage examples, and fixed index file.
* 1.2.1
    * FIX: Forgot to change index.js file back to normal...
* 1.2.2
    * UPDATE: Fixed some documentation.


[https://github.com/nedinator/djs-commands](https://github.com/nedinator/djs-commands)

## Contributing

1. Fork it (<https://github.com/nedinator/djs-commands/fork>)
2. Create your feature branch (`git checkout -b feature/fooBar`)
3. Commit your changes (`git commit -am 'Add some fooBar'`)
4. Push to the branch (`git push origin feature/fooBar`)
5. Create a new Pull Request

<!-- Markdown link & img dfn's -->
[npm-image]: https://img.shields.io/npm/v/djs-commands.svg?style=flat-square
[npm-url]: https://www.npmjs.com/package/djs-commands
[npm-downloads]: https://img.shields.io/npm/dt/djs-commands.svg