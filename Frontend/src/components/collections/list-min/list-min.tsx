import { CardCollMin } from '..';
import styles from './list-min.module.scss';

type props = {
    theme: 'light' | 'dark';
    items?: string[];
};

export function ListMinCollections(props: props) {
    const { theme } = props;
    return (
        <ul className={styles.list}>
            <li className={styles.item}>
                <CardCollMin theme={theme} />
            </li>
            <li className={styles.item}>
                <CardCollMin theme={theme} />
            </li>
            <li className={styles.item}>
                <CardCollMin theme={theme} />
            </li>
        </ul>
    );
}
