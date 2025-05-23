import { CookingStep } from 'types';
import styles from './steps.module.scss';

type props = {
    steps: CookingStep[];
};

export function Steps(props: props) {
    const { steps } = props;

    return (
        <div className={styles.mainContainer}>
            <h3 className={styles.h3}>Приготовление</h3>
            <div className={styles.stepsContainer}>
                {steps.map((step) => (
                    <div className={styles.step} key={step.step}>
                        <p className={styles.title}>
                            Шаг {step.step}: {step.title}
                        </p>
                        <p className={styles.text}>{step.description}</p>
                    </div>
                ))}
            </div>
        </div>
    );
}
