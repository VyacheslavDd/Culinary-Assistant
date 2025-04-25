import { LayoutHeader } from 'components/layout';
import styles from './coll-header.module.scss';
import fav from '../../../assets/svg/coll_fav_dark.svg';
import photo from '../../../assets/img/Photo.png';

export function OtherCollHeader() {
    return (
        <div className={styles.mainContainer}>
            <LayoutHeader>
                <div className={styles.container}>
                    <div className={styles.info}>
                        <div className={styles.name}>
                            <p className={styles.title}>Для новичков</p>
                        </div>
                        <div className={styles.buttons}>
                            <div className={styles.profile}>
                                <img src={photo} alt='profile' className={styles.img} />
                                AnnyLi
                            </div>
                            <div className={styles.fav}>
                                <img
                                    src={fav}
                                    alt='favorite'
                                    className={styles.icon}
                                />
                                <span>224</span>
                            </div>
                        </div>
                    </div>
                    <div className={styles.description}>
                        <div className={styles.shareContainer}>
                            <button>
                                <span className={`icon ${styles.like}`}></span>
                                Нравится
                            </button>
                            <button>
                                <span className={`icon ${styles.share}`}></span>
                                Поделиться
                            </button>
                        </div>
                        <p>Опубликована 11 апреля 2025</p>
                    </div>
                </div>
            </LayoutHeader>
        </div>
    );
}
