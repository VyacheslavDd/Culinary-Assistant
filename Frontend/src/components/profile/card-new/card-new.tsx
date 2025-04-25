import { useState } from 'react';
import styles from './card-new.module.scss';
import { ReactComponent as CrossIcon } from '../../../assets/svg/cross.svg';
import { ReactComponent as PlusIcon } from '../../../assets/svg/button_plus.svg';

export function CardNew() {
    const [isOpen, setIsOpen] = useState(false);

    const handleOpen = () => setIsOpen(true);
    const handleClose = () => setIsOpen(false);

    return (
        <div className={styles.mainContainer}>
            <div className={styles.container}>
                {isOpen ? (
                    <div className={styles.content}>
                        <div className={styles.cross} onClick={handleClose}>
                            <CrossIcon className={styles.iconCross} />
                        </div>
                        <div className={styles.inputContainer}>
                            <input
                                type='text'
                                className={styles.input}
                                placeholder='Название подборки'
                            />
                            <button className={`button ${styles.buttonCreate}`}>
                                Создать
                            </button>
                        </div>
                    </div>
                ) : (
                    <div className={styles.plusWrapper} onClick={handleOpen}>
                        <PlusIcon className={styles.iconPlus} />
                    </div>
                )}
            </div>
        </div>
    );
}
