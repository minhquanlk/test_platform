/**
 *
 * QuizDetail
 *
 * Quiz.Detail page content scripts. Initialized from scripts.js file.
 *
 *
 */

//class QuizDetail {
//  constructor() {
//    this._initTimer();
//  }

//  _initTimer() {
//    if (typeof Countdown !== 'undefined') {
//        var minutes = parseFloat(document.getElementById('timer').getAttribute('minutes'));
//        var seconds = (minutes - Math.floor(minutes)) * 60;
//        var total = parseInt(Math.floor(minutes) * 60 + seconds, 10);
//        var dateToCount = new Date(new Date().setSeconds(new Date().getSeconds() + total + 1)); 
//      var countdown = new Countdown({
//        selector: '#timer',
//        leadingZeros: true,
//        msgBefore: '',
//        msgAfter: '',
//        msgPattern: `
//                      <div class="row gx-5">
//                          <div class="col-auto">
//                              <div class="display-5 text-primary mb-1"><b>{minutes}</b></div>
//                              <div>Minutes</div>
//                          </div>
//                          <div class="col-auto">
//                              <div class="display-5 text-primary mb-1"><b>{seconds}</b></div>
//                              <div>Seconds</div>
//                          </div>
//                      </div>`,
//         dateEnd: dateToCount,
//      });

//    }
//  }
//}

//function startCountdown() {
//    var countdownMinutes = parseInt(document.getElementById('timer').getAttribute('minutes'));
//    var countdownSeconds = parseInt(document.getElementById('timer').getAttribute('seconds'));
//    function updateCountdown() {
//        var minutesDisplay = countdownMinutes < 10 ? '0' + countdownMinutes : countdownMinutes;
//        var secondsDisplay = countdownSeconds < 10 ? '0' + countdownSeconds : countdownSeconds;

//        document.getElementById('minutes').innerHTML = '<b>' + minutesDisplay + '</b>';
//        document.getElementById('seconds').innerHTML = '<b>' + secondsDisplay + '</b>';

//        if (countdownMinutes === 0 && countdownSeconds === 0) {
//            clearInterval(countdownInterval);
//            document.getElementById("testForm").submit();
//        } else {
//            if (countdownSeconds === 0) {
//                countdownMinutes--;
//                countdownSeconds = 59;
//            } else {
//                countdownSeconds--;
//            }
//        }
//    }

//    updateCountdown(); // Initial update

//    var countdownInterval = setInterval(updateCountdown, 1000);
//}

//startCountdown();

function startCountdown() {
    var countdownMinutes = parseInt(document.getElementById('timer').getAttribute('minutes'));
    var countdownSeconds = parseInt(document.getElementById('timer').getAttribute('seconds'));
    var changeMinutes = (59 - countdownMinutes)*1000*60;
    var changeSeconds = (59 - countdownSeconds)*1000;
    let start = new Date();
    function updateCountdown() {
        var minutesDisplay = countdownMinutes < 10 ? '0' + countdownMinutes : countdownMinutes;
        var secondsDisplay = countdownSeconds < 10 ? '0' + countdownSeconds : countdownSeconds;

        document.getElementById('minutes').innerHTML = '<b>' + minutesDisplay + '</b>';
        document.getElementById('seconds').innerHTML = '<b>' + secondsDisplay + '</b>';
        let current = new Date();
        let count = +current - +start + changeSeconds + changeMinutes;
        let s = Math.floor((count / 1000)) % 60;
        let m = Math.floor((count / 60000)) % 60;
        if (countdownMinutes <= 0 && countdownSeconds <= 0) {
            clearInterval(countdownInterval);
            document.getElementById("testForm").submit();
        } else {

            countdownSeconds = 59 - s;

            countdownMinutes = 59 - m;
        }

    }

    updateCountdown(); // Initial update

    var countdownInterval = setInterval(updateCountdown, 200);
}

startCountdown();