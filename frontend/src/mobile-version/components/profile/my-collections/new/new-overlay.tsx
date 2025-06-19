import styles from './new-overlay.module.scss';
import { ReactComponent as CheckIcon } from '../../../../../assets/svg/check_black.svg';
import { useState } from 'react';

type props = {
    onClick: (name: string) => void;
};

export function NewCollectionOverlay(props: props) {
    const { onClick } = props;
    const [title, setTitle] = useState('');

    return (
        <div className={styles.mainContainer}>
            <p>Новая подборка</p>
            <div className={styles.inputContainer}>
                <input
                    onChange={(e) => setTitle(e.target.value)}
                    value={title}
                    className={styles.input}
                    type='text'
                    placeholder='Название'
                />
                <button
                    className={styles.button}
                    onClick={() => onClick(title)}
                >
                    <CheckIcon className={styles.check} />
                </button>
            </div>
        </div>
    );
}
