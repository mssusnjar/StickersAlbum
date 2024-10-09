import { useState, useEffect } from 'react';

const CountdownTimer = ({targetTime, onZero} : {targetTime: number, onZero: () => void}) => {
  const [timeRemaining, setTimeRemaining] = useState(targetTime - new Date().getTime());
  const time = new Date(timeRemaining);
  const displayHours = time.getUTCHours() > 0;

  useEffect(() => {
    const timerInterval = setInterval(() => {
      const newTimeRemaining = targetTime - new Date().getTime();
      if (newTimeRemaining <= 0) onZero();
      setTimeRemaining(newTimeRemaining > 0 ? newTimeRemaining : 0)
    }
    , 1000);

    return () => clearInterval(timerInterval);
  }, []);

  return <span>{`${displayHours ? `${time.getUTCHours()}h ` : ""}${time.getUTCMinutes()}min ${displayHours ? "" : `${time.getUTCSeconds()}s`}`}</span>
};

export default CountdownTimer;