var body = document.getElementsByTagName("body")[0];

window.addEventListener("message", function(event) {
        if (event.data.action == "display") {
            type = event.data.type;

            document.getElementById("Main").style = "opacity: 1;";
        }
        else if(event.data.action == "AddTimer"){
            AddTimer(event.data.seconds);
        }
        else if(event.data.action == "RemoveTimer"){
            RemoveAllTimers();
        }
});

function AddTimer(seconds){
    miliseconds = seconds*1000;
    var id = Math.random();

    var timer = `<div class="flex h-full w-full items-center justify-center">
        <div class="h-24 w-24 rounded-[100px] border-t-2 border-b-2 border-l-2 border-r-2 border-l-slate-900 border-r-slate-900 flex justify-center items-center animate-spin"></div>
        <div id="${id}" class="text-white absolute">${seconds}</div>
        </div>`;
    body.innerHTML += timer;
    //body.appendChild(timer);
    var timerSeconds = document.getElementById(id);
    setInterval(Updater, 1000);
    
    function Updater(){
        if(document.getElementById(id) == null) { clearInterval(Updater); return;}
        timerSeconds.innerText = parseInt(timerSeconds.innerText)-1;
        if(parseInt(timerSeconds.innerText)-1 < 0){ clearInterval(Updater); RemoveAllTimers(); }
    }
}

function RemoveAllTimers(){
    body = document.getElementsByTagName("body")[0];
    var elements = Array.from(body.getElementsByTagName("div"));
    elements.forEach(element => {
        element.remove();       
    });
}