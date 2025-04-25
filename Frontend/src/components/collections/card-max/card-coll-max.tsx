import styles from './card-coll-max.module.scss';
import favIcon from '../../../assets/svg/coll_fav_dark.svg';
import photo from '../../../assets/img/Photo.png';
import { useNavigate } from 'react-router-dom';

export function CardCollMax() {
    const navigate = useNavigate();

    const handleClick = () => {
        navigate('/collection');
    };
    return (
        <div className={styles.mainContainer} onClick={handleClick}>
            <div className={styles.info}>
                <div className={styles.title}>
                    <div className={styles.name}>Подборка 1</div>
                    <p>от MaryDanilova</p>
                </div>

                <div className={styles.rating}>
                    <span
                        className={styles.icon}
                        style={{ backgroundImage: `url(${favIcon})` }}
                    ></span>
                    <span>224</span>
                </div>
            </div>
            <ul className={styles.list}>
                <li className={styles.item}>
                    <div className={styles.title}>
                        Овсянка на кокосовом молоке
                    </div>
                    <img src={photo} alt='receipt' className={styles.img} />
                </li>
                <li className={styles.item}>
                    <div className={styles.title}>
                        Овсянка на кокосовом молоке
                    </div>
                    <img src={photo} alt='receipt' className={styles.img} />
                </li>
                <li className={styles.item}>
                    <div className={styles.title}>
                        Овсянка на кокосовом молоке
                    </div>
                    <img src={photo} alt='receipt' className={styles.img} />
                </li>
                <li className={styles.item}>
                    <div className={styles.title}>
                        Овсянка на кокосовом молоке
                    </div>
                    <img src={photo} alt='receipt' className={styles.img} />
                </li>
                <li className={styles.item}>
                    <div className={styles.title}>
                        Овсянка на кокосовом молоке
                    </div>
                    <img src={photo} alt='receipt' className={styles.img} />
                </li>
                <li className={styles.item}>
                    <div className={styles.title}>
                        Овсянка на кокосовом молоке
                    </div>
                    <img src={photo} alt='receipt' className={styles.img} />
                </li>
                <li>
                    <div className={styles.more}>+8</div>
                </li>
            </ul>
            <p className={styles.date}>Опубликована 11 апреля 2025</p>
        </div>
    );
}
