import styles from './card-coll-min.module.scss';
import photo from '../../../assets/img/Photo.png';
import favIconLight from '../../../assets/svg/coll_fav_light.svg';
import favIconDark from '../../../assets/svg/coll_fav_dark.svg';
import { useNavigate } from 'react-router-dom';

type PropsCardCollMin = {
    theme: 'dark' | 'light';
};

export function CardCollMin(props: PropsCardCollMin) {
    const { theme } = props;
    const iconUrl = theme === 'light' ? favIconLight : favIconDark;

    const navigate = useNavigate();

    const handleClick = () => {
        navigate('/collection');
    };

    return (
        <div className={styles.mainContainer} onClick={handleClick}>
            <div className={styles.title}>Подборка 1</div>
            <div className={styles.list}>
                <div className={styles.item}>
                    <img src={photo} alt='receipt' className={styles.img} />
                </div>
                <div className={styles.item}></div>
                <div className={styles.item}></div>
                <div className={styles.item}></div>
                <div className={styles.item}></div>
                <div className={styles.item}></div>
            </div>
            <div className={`${styles.info} ${theme}`}>
                <p className={styles.author}>Автор: Мария Данилова</p>
                <div className={styles.rating}>
                    <span
                        className={styles.icon}
                        style={{ backgroundImage: `url(${iconUrl})` }}
                    ></span>
                    <span>224</span>
                </div>
            </div>
        </div>
    );
}
