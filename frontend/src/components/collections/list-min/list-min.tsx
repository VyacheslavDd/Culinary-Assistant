import { ShortCollection } from 'types/short-collection.type';
import { CardCollMin } from '..';
import styles from './list-min.module.scss';

type props = {
    theme: 'light' | 'dark';
    items?: ShortCollection[];
};

export function ListMinCollections(props: props) {
    const { theme, items = [] } = props;
    return (
        <ul className={styles.list}>
            {items.map((item) => (
                <li className={styles.item} key={item.id}>
                    <CardCollMin theme={theme} data={item} />
                </li>
            ))}
        </ul>
    );
}
