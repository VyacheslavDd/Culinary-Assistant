import { LayoutHeader } from 'components/layout';
import styles from './coll-header.module.scss';
import { Toggle } from 'components/common';
import fav from '../../../assets/svg/coll_fav_dark.svg';
import edit from '../../../assets/svg/edit.svg';

export function MyCollHeader() {
    return (
        <div className={styles.mainContainer}>
            <LayoutHeader>
                <div className={styles.container}>
                    <div className={styles.info}>
                        <div className={styles.name}>
                            <p className={styles.title}>Подборка 1</p>
                            <div className={styles.color}></div>
                        </div>
                        <div className={styles.buttons}>
                            <button className={styles.button}>
                                <img src={edit} alt='edit' />
                                Изм.
                            </button>
                            <div className={styles.publish}>
                                <Toggle isActive={false} />
                                <span>Опубликовать</span>
                            </div>
                            <div className={styles.fav}> 
                                <img src={fav} alt='favorite' className={styles.icon} />
                                <span>0</span>
                            </div>
                        </div>
                    </div>
                    <div className={styles.shareContainer}>
                        <button className='button'>
                            <span className={`icon ${styles.share}`}></span>
                            Поделиться
                        </button>
                        <button className={`button ${styles.deleteButton} `}>
                            <span className={`icon ${styles.delete}`}></span>
                            Удалить подборку
                        </button>
                    </div>
                </div>
            </LayoutHeader>
        </div>
    );
}
