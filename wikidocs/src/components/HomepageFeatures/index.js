import clsx from 'clsx';
import Heading from '@theme/Heading';
import styles from './styles.module.css';

const FeatureList = [
  {
    title: '압도적인 라인 절약',
    Svg: require('@site/static/img/undraw_docusaurus_mountain.svg').default,
    description: (
      <>
        반복적인 R3 래퍼/구독/정리 코드를 Source Generator가 자동으로
        생성하여 보일러플레이트를 크게 줄입니다.
      </>
    ),
  },
  {
    title: '컴파일 타임 안전성',
    Svg: require('@site/static/img/undraw_docusaurus_tree.svg').default,
    description: (
      <>
        R3Gen 진단이 partial, Awake/OnDestroy 호출 누락, 타입 불일치 등
        실수를 빌드 전에 잡아줍니다.
      </>
    ),
  },
  {
    title: '짧고 읽기 쉬운 코드',
    Svg: require('@site/static/img/undraw_docusaurus_react.svg').default,
    description: (
      <>
        Attribute 중심으로 선언하고 도메인 로직에만 집중할 수 있어
        팀 전체 유지보수 비용이 내려갑니다.
      </>
    ),
  },
];

function Feature({Svg, title, description}) {
  return (
    <div className={clsx('col col--4')}>
      <div className="text--center">
        <Svg className={styles.featureSvg} role="img" />
      </div>
      <div className="text--center padding-horiz--md">
        <Heading as="h3">{title}</Heading>
        <p>{description}</p>
      </div>
    </div>
  );
}

export default function HomepageFeatures() {
  return (
    <section className={styles.features}>
      <div className="container">
        <div className="row">
          {FeatureList.map((props, idx) => (
            <Feature key={idx} {...props} />
          ))}
        </div>
      </div>
    </section>
  );
}
